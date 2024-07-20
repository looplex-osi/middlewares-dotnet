using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;

public interface IAuthorizationService
{
    Task CreateAccessToken(IDefaultContext context, CancellationToken cancellationToken);
}