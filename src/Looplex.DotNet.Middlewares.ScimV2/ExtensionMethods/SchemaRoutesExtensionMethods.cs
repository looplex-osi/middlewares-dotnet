using System.Net;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class SchemaRoutesExtensionMethods
{
    private const string Resource = "/schemas";

    internal static MiddlewareDelegate GetMiddleware()
        => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<ISchemaService>();

        await service.GetAllAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    internal static MiddlewareDelegate GetByIdMiddleware()
        => async (context, cancellationToken, _) =>
    {
        HttpContext httpContext = context.State.HttpContext;
        var service = httpContext.RequestServices.GetRequiredService<ISchemaService>();

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;

        await service.GetByIdAsync(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    public static void UseSchemaRoute(this IEndpointRouteBuilder app, string[] services)
    {
        List<MiddlewareDelegate> getMiddlewares =
        [
            CoreMiddlewares.PaginationMiddleware,
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
            $"{Resource}/{{id}}",
            new RouteBuilderOptions
            {
                Services = services,
                Middlewares = getByIdMiddlewares.ToArray()
            });
    }
}