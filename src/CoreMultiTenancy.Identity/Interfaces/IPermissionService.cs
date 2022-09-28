using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Handles authorization checks based around Permissions
    /// </summary>
    public interface IPermissionService
    {
        /// <returns>
        /// Whether the User has all of the given Permissions, 
        /// or false if either doesn't exist or the User has no access.
        /// </returns>
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params string[] perms);

        Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId);

        /// <returns>A list of permission objects with populated PermissionCategories</returns>
        Task<List<Permission>> GetAllPermissionsAsync();
        /// <returns>A list of all PermissionCategories with populated Permissions</returns>
        Task<List<PermissionCategory>> GetAllPermissionCategoriesAsync();
    }
}