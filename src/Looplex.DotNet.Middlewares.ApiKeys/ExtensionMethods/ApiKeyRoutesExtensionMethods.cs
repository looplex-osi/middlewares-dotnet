using Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ApiKeys.Domain.Entities.ClientCredentials;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ApiKeys.ExtensionMethods;

public static class ApiKeyRoutesExtensionMethods
{
    public static Task UseApiKeyRoutesAsync(this IEndpointRouteBuilder app,
        string jsonSchemaId,
        string resource = "api-keys",
        ScimV2RouteOptions? options = null,
        CancellationToken? cancellationToken = null)
    {
        return app.UseScimV2RoutesAsync<ClientCredential, IApiKeyService>(
            resource,
            jsonSchemaId,
            options ?? new ScimV2RouteOptions(),
            cancellationToken ?? CancellationToken.None);
    }
}