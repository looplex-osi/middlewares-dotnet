using System.Net;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class SchemaRoutesExtensionMethods
{
    private const string Resource = "/Schemas";

    internal static MiddlewareDelegate GetMiddleware()
        => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<ISchemaService>();

        RoutesExtensionMethods.MapRequestParamsToContext(context, httpContext);

        await service.GetAllAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    internal static MiddlewareDelegate GetByIdMiddleware()
        => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<ISchemaService>();

        RoutesExtensionMethods.MapRequestParamsToContext(context, httpContext);

        await service.GetByIdAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    public static void UseSchemaRoute(this IEndpointRouteBuilder app, string[] services)
    {
        List<MiddlewareDelegate> getMiddlewares =
        [
            ScimV2Middlewares.PaginationMiddleware,
            GetMiddleware()
        ];
        app.MapGet(
            Resource,
            new RouteBuilderOptions
            {
                Services = services,
                Middlewares = getMiddlewares.ToArray()
            });

        List<MiddlewareDelegate> getByIdMiddlewares =
        [
            GetByIdMiddleware()
        ];
        app.MapGet(
            $"{Resource}/{{schemaId}}",
            new RouteBuilderOptions
            {
                Services = services,
                Middlewares = getByIdMiddlewares.ToArray()
            });
    }
}