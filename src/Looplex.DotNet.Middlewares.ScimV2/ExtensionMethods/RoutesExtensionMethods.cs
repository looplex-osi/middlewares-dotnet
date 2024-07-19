using Looplex.DotNet.Middlewares.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class RoutesExtensionMethods
{
    private static MiddlewareDelegate GetMiddleware<TService>(
        Action<IDefaultContext, HttpContext>? customAction = null)
        where TService : ICrudService => new(async (context, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        customAction?.Invoke(context, httpContext);

        await service.GetAllAsync(context);

        await httpContext.Response.WriteAsJsonAsync(context.Result);
    });

    private static MiddlewareDelegate GetByIdMiddleware<TService>(
        Action<IDefaultContext, HttpContext>? customAction = null)
        where TService : ICrudService => new(async (context, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;
        customAction?.Invoke(context, httpContext);

        await service.GetByIdAsync(context);

        await httpContext.Response.WriteAsJsonAsync(context.Result);
    });

    private static MiddlewareDelegate PostMiddleware<TService>(
        string resource,
        Action<IDefaultContext, HttpContext>? customAction = null)
        where TService : ICrudService => new(async (context, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync();

        customAction?.Invoke(context, httpContext);
        await service.CreateAsync(context);
        var id = context.Result;

        httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        httpContext.Response.Headers.Location = $"{resource}/{id}";
    });

    private static MiddlewareDelegate DeleteMiddleware<TService>(
        Action<IDefaultContext, HttpContext>? customAction = null)
        where TService : ICrudService => new(async (context, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;
        customAction?.Invoke(context, httpContext);

        await service.DeleteAsync(context);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    });

    public static void UseScimV2Routes<TResource, TService>(
        this IEndpointRouteBuilder app,
        ScimV2RouteOptions options)
        where TResource : Resource
        where TService : ICrudService
    {
        var resourceType = typeof(TResource).Name;
        var resource = resourceType[0].ToString().ToLower() + resourceType[1..];
        var tag = resourceType;

        var getBuiler = app.MapGet(
                resource,
                new RouteBuilderOptions
                {
                    Services = options.OptionsForGet.Services,
                    Middlewares =
                    [
                        AuthenticationMiddlewares.AuthenticateMiddleware,
                        CoreMiddlewares.PaginationMiddleware,
                        GetMiddleware<TService>(options.OptionsForGet.CustomAction)
                    ]
                })
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags(tag);
        options.OptionsForGet.AfterMapAction?.Invoke(getBuiler);

        var getByIdBuilder = app.MapGet(
                $"{resource}/{{id}}",
                new RouteBuilderOptions
                {
                    Services = options.OptionsForGetById.Services,
                    Middlewares =
                    [
                        AuthenticationMiddlewares.AuthenticateMiddleware,
                        GetByIdMiddleware<TService>(options.OptionsForGetById.CustomAction)
                    ]
                })
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
            .Produces(StatusCodes.Status401Unauthorized);
        options.OptionsForGetById.AfterMapAction?.Invoke(getByIdBuilder);

        var postBuilder = app.MapPost(
                resource,
                new RouteBuilderOptions
                {
                    Services = options.OptionsForPost.Services,
                    Middlewares = [
                        AuthenticationMiddlewares.AuthenticateMiddleware,
                        PostMiddleware<TService>(resource, options.OptionsForPost.CustomAction)
                    ]
                })
            .WithTags(tag)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status201Created);
        options.OptionsForPost.AfterMapAction?.Invoke(postBuilder);

        var deleteBuilder = app.MapDelete(
                $"{resource}/{{id}}",
                new RouteBuilderOptions
                {
                    Services = options.OptionsForDelete.Services,
                    Middlewares = [
                        AuthenticationMiddlewares.AuthenticateMiddleware,
                        DeleteMiddleware<TService>(options.OptionsForDelete.CustomAction)
                    ]
                })
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
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status204NoContent);
        options.OptionsForDelete.AfterMapAction?.Invoke(deleteBuilder);

    }
}