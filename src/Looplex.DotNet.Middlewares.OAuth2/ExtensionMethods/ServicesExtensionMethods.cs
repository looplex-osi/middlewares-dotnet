﻿using Casbin;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;

public static class ServicesExtensionMethods
{
    public static void AddOAuth2Services(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationServiceFactory, AuthorizationServiceFactory>();
        services.AddSingleton<IRBACService, CasbinRBACService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddTransient<TokenExchangeAuthorizationService>();
        services.AddTransient<ClientCredentialsAuthorizationService>();

        //provisorio source do casbin
        var enforcer = new Enforcer("model.conf", "policy.csv");
        services.AddSingleton(enforcer);
    }
}