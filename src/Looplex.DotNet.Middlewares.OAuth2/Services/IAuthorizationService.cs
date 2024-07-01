using Looplex.DotNet.Middlewares.OAuth2.DTOs;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Middlewares.OAuth2.Services
{
    public interface IAuthorizationService
    {
        Task CreateAccessToken(IDefaultContext context);
    }
}