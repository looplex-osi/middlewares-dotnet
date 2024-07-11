using Looplex.DotNet.Middlewares.ScimV2.DTOs.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods
{
    public static class GroupRoutesExtensionMethods
    {
        public static void UseGroupRoutes(this IEndpointRouteBuilder app, ScimV2RouteOptions? options = null)
        {
            app.UseScimV2Routes<Group, GroupReadDTO, GroupWriteDTO, IGroupService>(options ?? new ScimV2RouteOptions());
        }
    }
}
