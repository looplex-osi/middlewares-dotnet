using Looplex.DotNet.Middlewares.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Core.Common.Utils;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class RoutesExtensionMethods
{
    private static MiddlewareDelegate GetMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        await service.GetAllAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    private static MiddlewareDelegate GetByIdMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;

        await service.GetByIdAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    private static MiddlewareDelegate PostMiddleware<TService>(
        string resource)
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync(cancellationToken);

        await service.CreateAsync(context, cancellationToken);
        var id = context.Result;

        httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        httpContext.Response.Headers.Location = $"{resource}/{id}";
    };
    
    private static MiddlewareDelegate PatchMiddleware<TService>(
        string resource)
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;
        
        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Operations = await reader.ReadToEndAsync(cancellationToken);

        await service.PatchAsync(context, cancellationToken);

        httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        httpContext.Response.Headers.Location = $"{resource}/{id}";
    };

    private static MiddlewareDelegate DeleteMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;

        await service.DeleteAsync(context, cancellationToken);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    };

    public static void UseScimV2Routes<TService>(
        this IEndpointRouteBuilder app,
        string resource,
        ScimV2RouteOptions options)
        where TService : ICrudService
    {
        List<MiddlewareDelegate> getMiddlewares = [
            AuthenticationMiddlewares.AuthenticateMiddleware, CoreMiddlewares.PaginationMiddleware];
        getMiddlewares.AddRange(options.OptionsForGet?.Middlewares ?? []);
        getMiddlewares.Add(GetMiddleware<TService>());
        app.MapGet(
            resource,
            new RouteBuilderOptions
            {
                Services = options.OptionsForGet?.Services ?? [],
                Middlewares = getMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> getByIdMiddlewares = [AuthenticationMiddlewares.AuthenticateMiddleware];
        getByIdMiddlewares.AddRange(options.OptionsForGetById?.Middlewares ?? []);
        getByIdMiddlewares.Add(GetByIdMiddleware<TService>());
        app.MapGet(
            $"{resource}/{{id}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForGetById?.Services ?? [],
                Middlewares = getByIdMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> postMiddlewares = [AuthenticationMiddlewares.AuthenticateMiddleware];
        postMiddlewares.AddRange(options.OptionsForPost?.Middlewares ?? []);
        postMiddlewares.Add(PostMiddleware<TService>(resource));
        app.MapPost(
            resource,
            new RouteBuilderOptions
            {
                Services = options.OptionsForPost?.Services ?? [],
                Middlewares = postMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> patchMiddlewares = [AuthenticationMiddlewares.AuthenticateMiddleware];
        patchMiddlewares.AddRange(options.OptionsForPatch?.Middlewares ?? []);
        patchMiddlewares.Add(PatchMiddleware<TService>(resource));
        app.MapPatch(
            resource,
            new RouteBuilderOptions
            {
                Services = options.OptionsForPatch?.Services ?? [],
                Middlewares = patchMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> deleteMiddlewares = [AuthenticationMiddlewares.AuthenticateMiddleware];
        deleteMiddlewares.AddRange(options.OptionsForDelete?.Middlewares ?? []);
        deleteMiddlewares.Add(DeleteMiddleware<TService>());
        app.MapDelete(
            $"{resource}/{{id}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForDelete?.Services ?? [],
                Middlewares = deleteMiddlewares.ToArray()
            });
    }
}