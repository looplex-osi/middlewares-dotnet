using System.Net;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class ResourceTypeRoutesExtensionMethods
{
    private const string Resource = "/ResourceTypes";

    internal static MiddlewareDelegate GetMiddleware()
        => async (context, _) =>
        {
            var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
            var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
            var service = httpContext.RequestServices.GetRequiredService<IResourceTypeService>();

            await service.GetAllAsync(context, cancellationToken);

            await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
        };

    internal static MiddlewareDelegate GetByIdMiddleware()
        => async (context, _) =>
        {
            var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
            var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
            var service = httpContext.RequestServices.GetRequiredService<IResourceTypeService>();

            await service.GetByIdAsync(context, cancellationToken);

            await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
        };

    public static void UseResourceTypeRoute(this IEndpointRouteBuilder app, string[] services)
    {
        List<MiddlewareDelegate> getMiddlewares =
        [
            ScimV2Middlewares.ScimV2ContextMiddleware, ScimV2Middlewares.PaginationMiddleware,
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
            ScimV2Middlewares.ScimV2ContextMiddleware, GetByIdMiddleware()
        ];
        app.MapGet(
            $"{Resource}/{{resourceTypeId}}",
            new RouteBuilderOptions
            {
                Services = services,
                Middlewares = getByIdMiddlewares.ToArray()
            });
    }
}