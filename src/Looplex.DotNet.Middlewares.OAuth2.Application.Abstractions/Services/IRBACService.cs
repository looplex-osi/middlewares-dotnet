
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services
{
    /// <summary>
    /// Provides role-based access control (RBAC) functionality for authorization checks.
    /// </summary>
    public interface IRbacService
    {
        /// <summary>
        /// Asynchronously checks if a user has permission to perform an action on a resource within a domain.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="domain">The domain/tenant context for the permission check</param>
        /// <param name="resource">The resource being accessed</param>
        /// <param name="action">The action being performed on the resource</param>
        /// <returns>True if the user has permission, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        Task<bool> CheckPermissionAsync(string userId, string domain, string resource, string action);
    }
}
