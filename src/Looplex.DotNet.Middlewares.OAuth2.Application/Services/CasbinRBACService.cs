using Casbin;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services
{
    public class CasbinRbacService(
    IEnforcer enforcer) : IRbacService
    {
        private readonly IEnforcer _enforcer = enforcer;

        public Task<bool> CheckPermissionAsync(string userId, string domain, string resource, string action)
        {
            return _enforcer.EnforceAsync(userId, domain, resource, action);
        }
    }

}
