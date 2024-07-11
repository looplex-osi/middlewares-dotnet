using Looplex.DotNet.Middlewares.ScimV2.DTOs.Users;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods
{
    public static class UserRoutesExtensionMethods
    {
        public static void UseUserRoutes(this IEndpointRouteBuilder app, ScimV2RouteOptions? options = null)
        {
            app.UseScimV2Routes<User, UserReadDTO, UserWriteDTO, IUserService>(options ?? new ScimV2RouteOptions());
        }
    }
}
