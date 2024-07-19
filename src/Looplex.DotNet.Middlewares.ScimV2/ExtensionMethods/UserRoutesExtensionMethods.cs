using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class UserRoutesExtensionMethods
{
    public static void UseUserRoutes(this IEndpointRouteBuilder app, ScimV2RouteOptions? options = null)
    {
        app.UseScimV2Routes<User, IUserService>(options ?? new ScimV2RouteOptions());
    }
}