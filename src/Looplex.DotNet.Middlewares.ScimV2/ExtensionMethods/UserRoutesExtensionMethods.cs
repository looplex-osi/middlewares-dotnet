using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class UserRoutesExtensionMethods
{
    public static Task UseUserRoutesAsync(this IEndpointRouteBuilder app,
        string jsonSchemaId,
        string resource = "users",
        ScimV2RouteOptions? options = null,
        CancellationToken? cancellationToken = null)
    {
        return app.UseScimV2RoutesAsync<User, IUserService>(
            resource,
            jsonSchemaId,
            options ?? new ScimV2RouteOptions(),
            cancellationToken ?? CancellationToken.None);
    }
}