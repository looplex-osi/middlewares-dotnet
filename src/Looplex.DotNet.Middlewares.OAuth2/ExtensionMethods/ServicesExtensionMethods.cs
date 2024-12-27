using Casbin;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;

public static class ServicesExtensionMethods
{
    public static void AddOAuth2Services(this IServiceCollection services, IEnforcer enforcer)
    {
        services.AddSingleton<IAuthorizationServiceFactory, AuthorizationServiceFactory>();
        services.AddSingleton<IRbacService, CasbinRbacService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddTransient<TokenExchangeAuthorizationService>();
        services.AddTransient<ClientCredentialsAuthorizationService>();
        services.AddSingleton(enforcer);
    }
}