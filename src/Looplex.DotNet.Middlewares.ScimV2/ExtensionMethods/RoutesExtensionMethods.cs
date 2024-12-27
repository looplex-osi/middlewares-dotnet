using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.OAuth2.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class RoutesExtensionMethods
{
    private static MiddlewareDelegate GetMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        MapRequestParamsToContext(context, httpContext);
        
        await service.GetAllAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    private static MiddlewareDelegate GetByIdMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        MapRequestParamsToContext(context, httpContext);

        await service.GetByIdAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    private static MiddlewareDelegate PostMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        MapRequestParamsToContext(context, httpContext);

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync(cancellationToken);

        await service.CreateAsync(context, cancellationToken);
        var id = context.Result;
        httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        httpContext.Response.Headers.Location = $"{httpContext.Request.Path.Value}/{id}";
    };
    
    private static MiddlewareDelegate PutMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        MapRequestParamsToContext(context, httpContext);

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync(cancellationToken);

        await service.UpdateAsync(context, cancellationToken);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    };

    private static MiddlewareDelegate PatchMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        MapRequestParamsToContext(context, httpContext);
        
        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Operations = await reader.ReadToEndAsync(cancellationToken);

        await service.PatchAsync(context, cancellationToken);

        // TODO: The server MUST return a 200 OK (and the model in the body)
        // if the "attributes" parameter is specified in the request.

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    };

    private static MiddlewareDelegate DeleteMiddleware<TService>()
        where TService : ICrudService => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        MapRequestParamsToContext(context, httpContext);
        
        await service.DeleteAsync(context, cancellationToken);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    };

    public static async Task UseScimV2RoutesAsync<T, TService>(
        this IEndpointRouteBuilder app,
        string route,
        string jsonSchemaId,
        ScimV2RouteOptions options,
        CancellationToken cancellationToken)
        where T : Resource
        where TService : ICrudService
    {
        var schemaService = app.ServiceProvider.GetRequiredService<ISchemaService>();
        var resourceTypeService = app.ServiceProvider.GetRequiredService<IResourceTypeService>();
        var contextFactory = app.ServiceProvider.GetRequiredService<IContextFactory>();
        var serviceProviderConfiguration = app.ServiceProvider.GetRequiredService<ServiceProviderConfiguration>();

        var context = contextFactory.Create([]);
        context.AsScimV2Context().RouteValues.Add("schemaId", jsonSchemaId);
        await schemaService.CreateAsync(context, cancellationToken);
        var resourceTypeId = typeof(T).Name;
        context.State.ResourceType = new ResourceType
        {
            Id = resourceTypeId,
            Name = options.ResourceTypeName ?? typeof(T).Name,
            Description = options.ResourceTypeDescription,
            Endpoint = route,
            Meta = new()
            {
                Location = new Uri($"/ResourceType/{resourceTypeId}"),
                ResourceType = typeof(T).Name,
            },
            Schema = jsonSchemaId,
            Schemas = [],
        };
        await resourceTypeService.CreateAsync(context, cancellationToken);
        serviceProviderConfiguration.Map.Add(new()
        {
            Type = typeof(T),
            Resource = route,
            Service = typeof(TService)
        });

        var id = $"{ToLowerFirstLetter(typeof(T).Name)}Id";
        
        List<MiddlewareDelegate> getMiddlewares = [
            AuthenticationMiddleware.AuthenticateMiddleware, AuthorizationMiddleware.AuthorizeMiddleware, ScimV2Middlewares.PaginationMiddleware];
        getMiddlewares.AddRange(options.OptionsForGet?.Middlewares ?? []);
        getMiddlewares.Add(GetMiddleware<TService>());
        app.MapGet(
            route,
            new RouteBuilderOptions
            {
                Services = options.OptionsForGet?.Services ?? [],
                Middlewares = getMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> getByIdMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware, AuthorizationMiddleware.AuthorizeMiddleware];
        getByIdMiddlewares.AddRange(options.OptionsForGetById?.Middlewares ?? []);
        getByIdMiddlewares.Add(GetByIdMiddleware<TService>());
        app.MapGet(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForGetById?.Services ?? [],
                Middlewares = getByIdMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> postMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware, AuthorizationMiddleware.AuthorizeMiddleware];
        postMiddlewares.AddRange(options.OptionsForPost?.Middlewares ?? []);
        postMiddlewares.Add(PostMiddleware<TService>());
        app.MapPost(
            route,
            new RouteBuilderOptions
            {
                Services = options.OptionsForPost?.Services ?? [],
                Middlewares = postMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> putMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware, AuthorizationMiddleware.AuthorizeMiddleware];
        putMiddlewares.AddRange(options.OptionsForPut?.Middlewares ?? []);
        putMiddlewares.Add(PutMiddleware<TService>());
        app.MapPut(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForPut?.Services ?? [],
                Middlewares = putMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> patchMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware, AuthorizationMiddleware.AuthorizeMiddleware];
        patchMiddlewares.AddRange(options.OptionsForPatch?.Middlewares ?? []);
        patchMiddlewares.Add(PatchMiddleware<TService>());
        app.MapPatch(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForPatch?.Services ?? [],
                Middlewares = patchMiddlewares.ToArray()
            });
        
        List<MiddlewareDelegate> deleteMiddlewares = [AuthenticationMiddleware.AuthenticateMiddleware, AuthorizationMiddleware.AuthorizeMiddleware];
        deleteMiddlewares.AddRange(options.OptionsForDelete?.Middlewares ?? []);
        deleteMiddlewares.Add(DeleteMiddleware<TService>());
        app.MapDelete(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForDelete?.Services ?? [],
                Middlewares = deleteMiddlewares.ToArray()
            });
    }

    public static void MapRequestParamsToContext(IContext context, HttpContext httpContext)
    {
        context.AsScimV2Context().RouteValues = httpContext.Request.RouteValues
            .Select(rv => new KeyValuePair<string, object?>(rv.Key, rv.Value))
            .ToDictionary();
        context.AsScimV2Context().Query = httpContext.Request.Query
            .Select(q => new KeyValuePair<string, string>(q.Key, q.Value.ToString())).ToDictionary();
        context.AsScimV2Context().Headers = httpContext.Request.Headers
            .Select(q => new KeyValuePair<string, string>(q.Key, q.Value.ToString())).ToDictionary();
    }
    
    private static string ToLowerFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input[1..];
    }
}