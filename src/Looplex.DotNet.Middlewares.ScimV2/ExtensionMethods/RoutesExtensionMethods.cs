using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.OAuth2.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

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
    
    private static MiddlewareDelegate PutMiddleware<TService>(
        string resource)
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        // TODO
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;
        
        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Operations = await reader.ReadToEndAsync(cancellationToken);

        await service.UpdateAsync(context, cancellationToken);

        // TODO: The server MUST return a 200 OK (and the model in the body)
        // if the "attributes" parameter is specified in the request.

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
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

        // TODO: The server MUST return a 200 OK (and the model in the body)
        // if the "attributes" parameter is specified in the request.

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
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

    public static async Task UseScimV2RoutesAsync<T, TService>(
        this IEndpointRouteBuilder app,
        string resource,
        string jsonSchemaId,
        ScimV2RouteOptions options,
        CancellationToken cancellationToken)
        where T : Resource
        where TService : ICrudService
    {
        var schemaService = app.ServiceProvider.GetRequiredService<ISchemaService>();
        var contextFactory = app.ServiceProvider.GetRequiredService<IContextFactory>();
        var context = contextFactory.Create([]);
        context.State.Id = jsonSchemaId;
        await schemaService.CreateAsync(context, cancellationToken);
        await schemaService.GetByIdAsync(context, cancellationToken);
        
        // Cache the default jsonschema for model validation on deserialization purposes
        Schemas.Add(typeof(T), (string)context.Result!);
        
        List<MiddlewareDelegate> getMiddlewares = [
            AuthenticationMiddleware.AuthenticateMiddleware, CoreMiddlewares.PaginationMiddleware];
        getMiddlewares.AddRange(options.OptionsForGet?.Middlewares ?? []);
        getMiddlewares.Add(GetMiddleware<TService>());
        app.MapGet(
            resource,
            new RouteBuilderOptions
            {
                Services = options.OptionsForGet?.Services ?? [],
                Middlewares = getMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> getByIdMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware];
        getByIdMiddlewares.AddRange(options.OptionsForGetById?.Middlewares ?? []);
        getByIdMiddlewares.Add(GetByIdMiddleware<TService>());
        app.MapGet(
            $"{resource}/{{id}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForGetById?.Services ?? [],
                Middlewares = getByIdMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> postMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware];
        postMiddlewares.AddRange(options.OptionsForPost?.Middlewares ?? []);
        postMiddlewares.Add(PostMiddleware<TService>(resource));
        app.MapPost(
            resource,
            new RouteBuilderOptions
            {
                Services = options.OptionsForPost?.Services ?? [],
                Middlewares = postMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> putMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware];
        putMiddlewares.AddRange(options.OptionsForPut?.Middlewares ?? []);
        putMiddlewares.Add(PutMiddleware<TService>(resource));
        app.MapPut(
            $"{resource}/{{id}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForPut?.Services ?? [],
                Middlewares = putMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> patchMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware];
        patchMiddlewares.AddRange(options.OptionsForPatch?.Middlewares ?? []);
        patchMiddlewares.Add(PatchMiddleware<TService>(resource));
        app.MapPatch(
            $"{resource}/{{id}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForPatch?.Services ?? [],
                Middlewares = patchMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> deleteMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware];
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