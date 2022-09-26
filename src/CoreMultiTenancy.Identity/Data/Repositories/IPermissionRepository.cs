using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IPermissionRepository
    {
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms);
        Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId);
        
        /// <summary>
        /// Sets the User's permissions based on the role id under a specific tenant.
        /// </summary>
        /// <returns>An Error on failure.</returns>
        Task<Option<Error>> SetUsersPermissionsAsync(Guid userId, Guid orgId, Guid roleId);

        /// <returns>A list of all visible permissions with populated PermissionCategory objects</returns>
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<PermissionCategory>> GetAllPermissionCategoriesAsync();
    }
}