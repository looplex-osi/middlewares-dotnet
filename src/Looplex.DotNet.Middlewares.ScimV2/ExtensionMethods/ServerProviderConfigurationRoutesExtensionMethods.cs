using System.Net;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class ServerProviderConfigurationRoutesExtensionMethods
{
    private const string Resource = "/ServiceProviderConfig";

    internal static MiddlewareDelegate GetMiddleware()
        => async (context, _) =>
    {
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        var config = httpContext.RequestServices.GetRequiredService<ServiceProviderConfiguration>();
        var result = config.ToJson();
        
        await httpContext.Response.WriteAsJsonAsync(result, HttpStatusCode.OK);
    };

    public static void UseServiceProviderConfigurationRoute(this IEndpointRouteBuilder app, string[] services)
    {
        List<MiddlewareDelegate> getMiddlewares =
        [
            GetMiddleware()
        ];
        app.MapGet(
            Resource,
            new RouteBuilderOptions
            {
                Services = services,
                Middlewares = getMiddlewares.ToArray()
            });
    }
}