using System.Net;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class BulkRoutesExtensionMethods
{
    private const string Resource = "/Bulk";

    internal static MiddlewareDelegate PostMiddleware()
        => async (context, _) =>
    {
        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var service = httpContext.RequestServices.GetRequiredService<IBulkService>();
        
        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Request = await reader.ReadToEndAsync(cancellationToken);

        var id = (string)httpContext.Request.RouteValues["id"]!;
        context.State.Id = id;

        await service.ExecuteBulkOperationsAsync(context);

        await httpContext.Response.WriteAsJsonAsync((string)context.Result!, HttpStatusCode.OK);
    };

    public static void UseBulkRoute(this IEndpointRouteBuilder app, string[] services)
    {
        List<MiddlewareDelegate> getByIdMiddlewares =
        [
            PostMiddleware()
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