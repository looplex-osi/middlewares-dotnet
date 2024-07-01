using AutoMapper;
using Looplex.DotNet.Core.Application.Abstractions.Pagination;
using Looplex.DotNet.Core.Common.Middlewares;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.OAuth2.DTOs;
using Looplex.DotNet.Middlewares.OAuth2.Entities;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Looplex.OpenForExtension.Context;
using Looplex.OpenForExtension.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods
{
    public static class ClientCredentialRoutesExtensionMethods
    {
        private const string RESOURCE = "/clients";
        private const string TAG = "Client";

        private readonly static MiddlewareDelegate GetMiddleware = new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IMapper mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            IClientCredentialService clientStorageService = httpContext.RequestServices.GetRequiredService<IClientCredentialService>();

            await clientStorageService.GetAll(context);

            var clients = (PaginatedCollection<IClient>)context.Result;
            var data = mapper.Map<PaginatedCollection<IClient>, PaginatedCollectionDTO<ClientReadDTO>>(clients);

            await httpContext.Response.WriteAsJsonAsync(data);
        });

        private readonly static MiddlewareDelegate GetByIdMiddleware = new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IMapper mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            IClientCredentialService clientStorageService = httpContext.RequestServices.GetRequiredService<IClientCredentialService>();

            Guid id = Guid.Parse((string)httpContext.Request.RouteValues["id"]!);

            context.State.Id = id;
            await clientStorageService.GetAsync(context);
            var client = (IClient)context.Result;

            var data = mapper.Map<IClient, ClientWriteDTO>(client);

            await httpContext.Response.WriteAsJsonAsync(data);
        });

        private readonly static MiddlewareDelegate PostMiddleware = new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IMapper mapper = httpContext.RequestServices.GetRequiredService<IMapper>();
            IClientCredentialService clientStorageService = httpContext.RequestServices.GetRequiredService<IClientCredentialService>();

            using StreamReader reader = new(httpContext.Request.Body);
            var clientWriteDTO = JsonSerializer.Deserialize<ClientWriteDTO>(await reader.ReadToEndAsync(), JsonUtils.HttpBodyConverter())!;

            var client = mapper.Map<ClientWriteDTO, Client>(clientWriteDTO);

            context.State.Client = client;
            await clientStorageService.CreateAsync(context);
            var id = (Guid)context.Result;

            httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            httpContext.Response.Headers.Location = $"{RESOURCE}/{id}";
        });

        private readonly static MiddlewareDelegate DeleteMiddleware = new(async (context, next) =>
        {
            HttpContext httpContext = context.State.HttpContext;
            IClientCredentialService clientStorageService = httpContext.RequestServices.GetRequiredService<IClientCredentialService>();

            Guid id = Guid.Parse((string)httpContext.Request.RouteValues["id"]!);
            
            context.State.Id = id;
            await clientStorageService.DeleteAsync(context);

            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
        });

        public static void UseClientCredentialRoutes(this IEndpointRouteBuilder app, IList<IPlugin> plugins)
        {
            var context = DefaultContext.Create(plugins, app.ServiceProvider);

            app.MapGet(
                RESOURCE,
                context,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    CoreMiddlewares.PaginationMiddleware,
                    GetMiddleware
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(TAG)
            .Produces<PaginatedCollectionDTO<ClientReadDTO>>(StatusCodes.Status200OK, JsonUtils.JsonContentTypeWithCharset);

            app.MapGet(
                $"{RESOURCE}/{{id}}",
                context,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    GetByIdMiddleware
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(TAG)
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
            .Produces<ClientReadDTO>(StatusCodes.Status200OK, JsonUtils.JsonContentTypeWithCharset);

            app.MapPost(
                RESOURCE,
                context,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    PostMiddleware
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(TAG)
            .Accepts<ClientWriteDTO>(JsonUtils.JsonContentTypeWithCharset)
            .Produces(StatusCodes.Status201Created);

            app.MapDelete(
                $"{RESOURCE}/{{id}}",
                context,
                [
                    AuthenticationMiddlewares.AuthenticateMiddleware,
                    DeleteMiddleware
                ],
                [StatusCodes.Status401Unauthorized])
            .WithTags(TAG)
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
