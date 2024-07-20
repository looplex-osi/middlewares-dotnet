using Looplex.DotNet.Middlewares.Clients.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.Clients.ExtensionMethods;

public static class ClientRoutesExtensionMethods
{
    public static void UseClientRoutes(this IEndpointRouteBuilder app, string resource = "clients", ScimV2RouteOptions? options = null)
    {
        app.UseScimV2Routes<IClientService>(resource, options ?? new ScimV2RouteOptions());
    }
}