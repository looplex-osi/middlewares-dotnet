using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class GroupRoutesExtensionMethods
{
    public static Task UseGroupRoutesAsync(this IEndpointRouteBuilder app,
        string jsonSchemaId,
        string resource = "users",
        ScimV2RouteOptions? options = null,
        CancellationToken? cancellationToken = null)
    {
        return app.UseScimV2RoutesAsync<Group, IGroupService>(
            resource,
            jsonSchemaId,
            options ?? new ScimV2RouteOptions(),
            cancellationToken ?? CancellationToken.None);
    }
}