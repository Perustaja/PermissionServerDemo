using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Overarching repository for dealing with UserOrganizationRoles, Roles, and RolePermissions.
    /// </summary>
    public interface IRoleRepository
    {
        /// <returns>A list of Roles including tenant-specific and global roles.</returns>
        Task<List<Role>> GetRolesOfOrgAsync(Guid orgId);

        /// <returns>An Option containing the Role if found.</returns>
        Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId);

        /// <summary>
        /// Adds a Role to be used by the specified organization.
        /// </summary>
        /// <returns>An Option containing the new Role if successful.</returns>
        Task<Option<Role>> AddRoleToOrgAsync(Guid orgId, Role role);

        /// <summary>
        /// Updates the Role.
        /// </summary>
        Task UpdateRoleOfOrgAsync(Role role);

        /// <summary>
        /// Attempts to delete the role. This will fail if any user has
        /// this role as their only role, and will not delete global roles.
        /// </summary>
        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> DeleteRoleOfOrgAsync(Role role);
    }
}