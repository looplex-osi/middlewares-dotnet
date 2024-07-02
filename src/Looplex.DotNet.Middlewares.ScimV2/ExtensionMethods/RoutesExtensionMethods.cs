using AutoMapper;
using Looplex.DotNet.Core.Application.Abstractions.Pagination;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Common.Middlewares;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.OAuth2;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using Looplex.OpenForExtension.Context;
using Looplex.OpenForExtension.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods
{
    public static class RoutesExtensionMethods
    {
        private static MiddlewareDelegate GetMiddleware<R, DTO, S>(
            Action<IDefaultContext, HttpContext>? customAction = null)
            where S : ICrudService => new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IMapper mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            S service = httpContext.RequestServices.GetRequiredService<S>();

            customAction?.Invoke(context, httpContext);

            await service.GetAll(context);

            var records = (PaginatedCollection<R>)context.Result;
            var data = mapper.Map<PaginatedCollection<R>, PaginatedCollectionDTO<DTO>>(records);

            await httpContext.Response.WriteAsJsonAsync(data);
        });

        private static MiddlewareDelegate GetByIdMiddleware<R, DTO, S>(
            Action<IDefaultContext, HttpContext>? customAction = null)
            where S : ICrudService => new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IMapper mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            S service = httpContext.RequestServices.GetRequiredService<S>();

            var id = (string)httpContext.Request.RouteValues["id"]!;
            context.State.Id = id;
            customAction?.Invoke(context, httpContext);

            await service.GetAsync(context);

            var client = (R)context.Result;
            var data = mapper.Map<R, DTO>(client);
            await httpContext.Response.WriteAsJsonAsync(data);
        });

        private static MiddlewareDelegate PostMiddleware<R, DTO, S>(
            string resource,
            Action<IDefaultContext, HttpContext>? customAction = null)
            where S : ICrudService => new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IMapper mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            S service = httpContext.RequestServices.GetRequiredService<S>();

            using StreamReader reader = new(httpContext.Request.Body);
            var clientWriteDTO = JsonSerializer.Deserialize<DTO>(await reader.ReadToEndAsync(), JsonUtils.HttpBodyConverter())!;
            var client = mapper.Map<DTO, R>(clientWriteDTO);
            context.State.Resource = client;

            customAction?.Invoke(context, httpContext);
            await service.CreateAsync(context);
            var id = context.Result;

            httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            httpContext.Response.Headers.Location = $"{resource}/{id}";
        });

        private static MiddlewareDelegate DeleteMiddleware<R, DTO, S>(
            Action<IDefaultContext, HttpContext>? customAction = null)
            where S : ICrudService => new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            S service = httpContext.RequestServices.GetRequiredService<S>();

            var id = (string)httpContext.Request.RouteValues["id"]!;
            context.State.Id = id;
            customAction?.Invoke(context, httpContext);

            await service.DeleteAsync(context);

            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
        });

        public static void UseScimV2Routes<R, RDTO, WDTO, S>(
            this IEndpointRouteBuilder app,
            IList<IPlugin> plugins,
            Action<IDefaultContext, HttpContext>? getCustomAction = null,
            Action<IDefaultContext, HttpContext>? getByIdCustomAction = null,
            Action<IDefaultContext, HttpContext>? postCustomAction = null,
            Action<IDefaultContext, HttpContext>? deleteCustomAction = null)
            where R : Resource
            where RDTO : notnull
            where WDTO : notnull
            where S : ICrudService
        {
            var resourceType = typeof(R).Name;
            string resource = resourceType[0].ToString().ToLower() + resourceType[1..];
            string tag = resourceType;

            app.MapGet(
                resource,
                plugins,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    CoreMiddlewares.PaginationMiddleware,
                    GetMiddleware<R, RDTO, S>(getCustomAction)
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(tag)
            .Produces<PaginatedCollectionDTO<RDTO>>(StatusCodes.Status200OK, JsonUtils.JsonContentTypeWithCharset);

            app.MapGet(
                $"{resource}/{{id}}",
                plugins,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    GetByIdMiddleware<R, RDTO, S>(getByIdCustomAction)
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(tag)
            .WithOpenApi(o =>
            {
                o.Parameters.Add(new OpenApiParameter
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Required = true,
                });
                return o;
            })
            .Produces<RDTO>(StatusCodes.Status200OK, JsonUtils.JsonContentTypeWithCharset);

            app.MapPost(
                resource,
                plugins,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    PostMiddleware<R, WDTO, S>(resource, postCustomAction)
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(tag)
            .Accepts<WDTO>(JsonUtils.JsonContentTypeWithCharset)
            .Produces(StatusCodes.Status201Created);

            app.MapDelete(
                $"{resource}/{{id}}",
                plugins,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    DeleteMiddleware<R, RDTO, S>(deleteCustomAction)
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(tag)
            .WithOpenApi(o =>
            {
                o.Parameters.Add(new OpenApiParameter
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Required = true,
                });
                return o;
            })
            .Produces(StatusCodes.Status204NoContent);
        }
    }
}
