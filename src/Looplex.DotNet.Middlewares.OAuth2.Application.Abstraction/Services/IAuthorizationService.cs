using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstraction.Services;

public interface IAuthorizationService
{
    Task CreateAccessToken(IDefaultContext context);
}