using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Identity.Entities;

namespace PermissionServerDemo.Identity.Interfaces
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