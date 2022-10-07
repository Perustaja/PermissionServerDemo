using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Interfaces;
using Perustaja.Polyglot.Option;

namespace PermissionServerDemo.Identity.Data.Repositories
{
    /// <summary>
    /// Handles Role management for Organizations, use IUserOrganizationRoleRepository for management of
    /// which Users have which Role within an Organization.
    /// </summary>
    public interface IRoleRepository : IRepository
    {
        /// <returns>A list of Roles including tenant-specific and global roles with populated Permissions.</returns>
        Task<List<Role>> GetRolesOfOrgAsync(Guid orgId);
        /// <returns>An Option containing the Role if found.</returns>
        Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId);
        Task<Role> GetGlobalDefaultOwnerRoleAsync();
        Task<Role> GetGlobalDefaultNewUserRoleAsync();
        /// <returns>The Role entity being tracked after add.</returns>
        Role Add(Guid orgId, Role role);
        /// <returns>The Role entity being tracked after update.</returns>
        Role Update(Role role);
        void Delete(Role role);
    }
}