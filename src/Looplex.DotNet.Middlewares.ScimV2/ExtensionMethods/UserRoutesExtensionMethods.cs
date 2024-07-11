using Looplex.DotNet.Middlewares.ScimV2.DTOs.Users;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Users;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods
{
    public static class UserRoutesExtensionMethods
    {
        public static void UseUserRoutes(this IEndpointRouteBuilder app, string[] services)
        {
            app.UseScimV2Routes<User, UserReadDTO, UserWriteDTO, IUserService>(new ScimV2RouteOptions
            {
                Services = services
            });
        }
    }
}
