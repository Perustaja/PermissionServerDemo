using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Handles authorization checks based around Permissions
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>Use instead of the plural form if checking a single Permission for performance.</summary>
        /// <returns>
        /// Whether the User has the given Permission, or false if either doesn't exist or the User has no access.
        /// </returns>
        Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm);
        
        /// <returns>
        /// Whether the User has all of the given Permissions, 
        /// or false if either doesn't exist or the User has no access.
        /// </returns>
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms);
    }
}