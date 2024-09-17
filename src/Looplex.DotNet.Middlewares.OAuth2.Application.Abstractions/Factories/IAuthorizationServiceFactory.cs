using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;

public interface IAuthorizationServiceFactory
{
    public IAuthorizationService GetService(GrantType grantType);
}