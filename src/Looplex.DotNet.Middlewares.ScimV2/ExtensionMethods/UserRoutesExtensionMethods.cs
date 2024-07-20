using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class UserRoutesExtensionMethods
{
    public static void UseUserRoutes(this IEndpointRouteBuilder app, string resource = "users", ScimV2RouteOptions? options = null)
    {
        app.UseScimV2Routes<IUserService>(resource, options ?? new ScimV2RouteOptions());
    }
}