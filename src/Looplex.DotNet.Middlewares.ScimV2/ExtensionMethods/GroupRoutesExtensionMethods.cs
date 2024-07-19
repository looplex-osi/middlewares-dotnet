using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class GroupRoutesExtensionMethods
{
    public static void UseGroupRoutes(this IEndpointRouteBuilder app, ScimV2RouteOptions? options = null)
    {
        app.UseScimV2Routes<Group, IGroupService>(options ?? new ScimV2RouteOptions());
    }
}