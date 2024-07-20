using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;

public interface IAuthorizationService
{
    Task CreateAccessToken(IContext context, CancellationToken cancellationToken);
}