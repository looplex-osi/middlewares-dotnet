using Casbin;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services
{
    public class CasbinRBACService(
    IEnforcer enforcer) : IRBACService
    {
        private readonly IEnforcer _enforcer = enforcer;


        public async Task<bool> CheckPermission(string userId, string domain, string resource, string action)
        {
            bool authorized = _enforcer.Enforce(userId, domain, resource, action);

            return authorized;
        }

    }

}
