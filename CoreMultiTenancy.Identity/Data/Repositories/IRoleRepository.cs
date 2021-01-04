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
    /// Handles Role management for Organizations, use IUserOrganizationRoleRepository for management of
    /// which Users have which Role within an Organization.
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
        /// Attempts to delete the Role. This will fail if any User has
        /// this Role as their only Role, and will not delete global Roles.
        /// </summary>
        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> DeleteRoleOfOrgAsync(Role role);
    }
}