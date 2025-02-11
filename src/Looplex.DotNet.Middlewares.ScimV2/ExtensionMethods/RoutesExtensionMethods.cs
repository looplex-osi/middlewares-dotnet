using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.OAuth2.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class RoutesExtensionMethods
{
    private static MiddlewareDelegate GetMiddleware<TService>()
        where TService : ICrudService => async (context, _) =>
    {
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        await service.GetAllAsync(context);

        await httpContext.Response.WriteAsJsonAsync((string)context.GetResult()!, HttpStatusCode.OK);
    };

    private static MiddlewareDelegate GetByIdMiddleware<TService>()
        where TService : ICrudService => async (context, _) =>
    {
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        await service.GetByIdAsync(context);

        await httpContext.Response.WriteAsJsonAsync((string)context.GetResult()!, HttpStatusCode.OK);
    };

    private static MiddlewareDelegate PostMiddleware<TService>()
        where TService : ICrudService => async (context, _) =>
    {
        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync(cancellationToken);

        await service.CreateAsync(context);
        var id = context.Result;
        httpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        httpContext.Response.Headers.Location = $"{httpContext.Request.Path.Value}/{id}";
    };

    private static MiddlewareDelegate PutMiddleware<TService>()
        where TService : ICrudService => async (context, _) =>
    {
        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync(cancellationToken);

        await service.UpdateAsync(context);

        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    };

    private static MiddlewareDelegate PatchMiddleware<TService>()
        where TService : ICrudService => async (context, _) =>
    {
        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Operations = await reader.ReadToEndAsync(cancellationToken);

        await service.PatchAsync(context);
        
        // The server MUST return a 200 OK (and the model in the body)
        // if the "attributes" parameter is specified in the request.
        var result = context.GetResult();
        if (!string.IsNullOrEmpty(context.GetQuery("excludedAttributes")) && result is not null)
            await httpContext.Response
                .WriteAsJsonAsync((string)result, HttpStatusCode.OK);
        else 
            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
    };

    private static MiddlewareDelegate DeleteMiddleware<TService>()
        where TService : ICrudService => async (context, _) =>
    {
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<TService>();

        await service.DeleteAsync(context);

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
        context.State.CancellationToken = cancellationToken;
        context.AsScimV2Context().RouteValues.Add("schemaId", jsonSchemaId);
        await schemaService.CreateAsync(context);
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
        await resourceTypeService.CreateAsync(context);
        serviceProviderConfiguration.Map.Add(new()
        {
            Type = typeof(T),
            Resource = route,
            Service = typeof(TService)
        });
        context.DisposeIfPossible();
        
        var id = $"{ToLowerFirstLetter(typeof(T).Name)}Id";

        List<MiddlewareDelegate> getMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, OAuth2Middlewares.AuthenticationMiddleware,
            ScimV2Middlewares.PaginationMiddleware
        ];
        getMiddlewares.AddRange(options.OptionsForGet?.Middlewares ?? []);
        getMiddlewares.Add(GetMiddleware<TService>());
        app.MapGet(
            route,
            new RouteBuilderOptions
            {
                Services = options.OptionsForGet?.Services ?? [],
                Middlewares = getMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> getByIdMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, OAuth2Middlewares.AuthenticationMiddleware
        ];
        getByIdMiddlewares.AddRange(options.OptionsForGetById?.Middlewares ?? []);
        getByIdMiddlewares.Add(GetByIdMiddleware<TService>());
        app.MapGet(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForGetById?.Services ?? [],
                Middlewares = getByIdMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> postMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, OAuth2Middlewares.AuthenticationMiddleware
        ];
        postMiddlewares.AddRange(options.OptionsForPost?.Middlewares ?? []);
        postMiddlewares.Add(PostMiddleware<TService>());
        app.MapPost(
            route,
            new RouteBuilderOptions
            {
                Services = options.OptionsForPost?.Services ?? [],
                Middlewares = postMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> putMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, OAuth2Middlewares.AuthenticationMiddleware
        ];
        putMiddlewares.AddRange(options.OptionsForPut?.Middlewares ?? []);
        putMiddlewares.Add(PutMiddleware<TService>());
        app.MapPut(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForPut?.Services ?? [],
                Middlewares = putMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> patchMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, OAuth2Middlewares.AuthenticationMiddleware
        ];
        patchMiddlewares.AddRange(options.OptionsForPatch?.Middlewares ?? []);
        patchMiddlewares.Add(PatchMiddleware<TService>());
        app.MapPatch(
            $"{route}/{{{id}}}",
            new RouteBuilderOptions
            {
                Services = options.OptionsForPatch?.Services ?? [],
                Middlewares = patchMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> deleteMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, OAuth2Middlewares.AuthenticationMiddleware
        ];
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

    private static string ToLowerFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input[1..];
    }
}