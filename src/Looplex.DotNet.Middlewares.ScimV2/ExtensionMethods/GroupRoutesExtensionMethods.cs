using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class GroupRoutesExtensionMethods
{
    public static void UseGroupRoutes(this IEndpointRouteBuilder app, string resource = "groups", ScimV2RouteOptions? options = null)
    {
        app.UseScimV2Routes<IGroupService>(resource, options ?? new ScimV2RouteOptions());
    }
}