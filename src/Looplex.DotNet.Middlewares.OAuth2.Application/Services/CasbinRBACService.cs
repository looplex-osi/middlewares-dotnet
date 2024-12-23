using Casbin;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Services
{
    public class CasbinRbacService(
    IEnforcer enforcer,
    ILogger<CasbinRbacService> logger) : IRbacService
    {
        private readonly IEnforcer _enforcer = enforcer;
        private readonly ILogger<CasbinRbacService> _logger = logger;

        public async Task<bool> CheckPermissionAsync(string userId, string domain, string resource, string action)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain));
            if (string.IsNullOrEmpty(resource)) throw new ArgumentNullException(nameof(resource));
            if (string.IsNullOrEmpty(action)) throw new ArgumentNullException(nameof(action));

            try
            {
                var authorized = await _enforcer.EnforceAsync(userId, domain, resource, action);

                _logger.LogInformation(
                "Permission check: User {UserId} in domain {Domain} accessing {Resource} with action {Action}. Result: {Result}",
                userId, domain, resource, action, authorized);

                return authorized;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                "Error checking permission for User {UserId} in domain {Domain} accessing {Resource} with action {Action}",
                userId, domain, resource, action);
                throw;
            }
        }
    }
}
