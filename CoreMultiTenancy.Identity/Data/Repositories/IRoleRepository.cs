using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Handles Role management for Organizations, use IUserOrganizationRoleRepository for management of
    /// which Users have which Role within an Organization.
    /// </summary>
    public interface IRoleRepository : IRepository
    {
        /// <returns>A list of Roles including tenant-specific and global roles.</returns>
        Task<List<Role>> GetRolesOfOrgAsync(Guid orgId);

        /// <returns>An Option containing the Role if found.</returns>
        Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId);

        /// <summary>
        /// Adds a Role to be used by the specified organization.
        /// </summary>
        /// <returns>The Role entity being tracked after add.</returns>
        Role AddRoleToOrg(Guid orgId, Role role);

        /// <summary>
        /// Updates the Role.
        /// </summary>
        /// <returns>The Role entity being tracked after update.</returns>
        Role UpdateRoleOfOrg(Role role);

        /// <summary>
        /// Attempts to delete the Role. This will fail if any User has
        /// this Role as their only Role, and will not delete global Roles.
        /// </summary>
        void DeleteRoleOfOrg(Role role);

        /// <returns>Whether a User has this Role as their only Role, and deleting it would leave them without one.</returns>
        Task<bool> RoleIsOnlyRoleForAnyUser(Role role);
    }
}