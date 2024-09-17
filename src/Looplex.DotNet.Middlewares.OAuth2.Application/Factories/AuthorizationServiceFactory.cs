using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Factories;

public class AuthorizationServiceFactory(IServiceProvider serviceProvider) : IAuthorizationServiceFactory
{
    public IAuthorizationService GetService(GrantType grantType)
    {
        return grantType switch
        {
            GrantType.TokenExchange => serviceProvider.GetRequiredService<TokenExchangeAuthorizationService>(),
            GrantType.ClientCredentials => serviceProvider.GetRequiredService<ClientCredentialsAuthorizationService>(),
            _ => throw new ArgumentOutOfRangeException(nameof(grantType), grantType, null)
        };    
    }
}