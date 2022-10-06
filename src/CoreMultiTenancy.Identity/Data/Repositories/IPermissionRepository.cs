using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IPermissionRepository
    {
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, string[] perms);
        Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId);
        /// <returns>A list of all permissions with populated PermissionCategory objects</returns>
        Task<List<Permission>> GetAllPermissionsAsync();
        Task<List<PermissionCategory>> GetAllPermissionCategoriesAsync();
    }
}