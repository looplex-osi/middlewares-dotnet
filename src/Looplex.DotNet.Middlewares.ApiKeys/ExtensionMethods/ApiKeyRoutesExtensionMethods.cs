using Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ApiKeys.ExtensionMethods;

public static class ApiKeyRoutesExtensionMethods
{
    public static void UseApiKeyRoutes(this IEndpointRouteBuilder app, string resource = "api-keys", ScimV2RouteOptions? options = null)
    {
        app.UseScimV2Routes<IApiKeyService>(resource, options ?? new ScimV2RouteOptions());
    }
}