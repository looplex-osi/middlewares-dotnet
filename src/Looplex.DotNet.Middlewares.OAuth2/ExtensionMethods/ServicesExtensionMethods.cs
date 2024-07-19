using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;

public static class ServicesExtensionMethods
{
    public static void AddOAuth2Services(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationService, AuthorizationService>();
        services.AddSingleton<IIdTokenService, IdTokenService>();
    }
}