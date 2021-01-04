using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Handles role management, tenant access, and tenant management. To reduce db calls
    /// some methods take ids as the context of the request means the existence of both the user and
    /// organization as well as the fact that the user has access to the organization have been
    /// verified via authorization. In these contexts, objects do not need to be instantiated to verify existence.
    /// </summary>
    public interface IOrganizationManager
    {
        #region OrganizationManagement
        /// <returns>An Option containing an Organization if found.</returns>
        Task<Option<Organization>> GetByIdAsync(Guid orgId);

        /// <summary>
        /// Adds the given tenant.
        /// </summary>
        /// <returns>An Option containing the new Organization if sucessful.</returns>
        Task<Option<Organization>> AddAsync(Organization o);

        /// <summary>
        /// Updates the given tenant.
        /// </summary>
        /// <returns>An Option containing the updated Organization if sucessful.</returns>
        Task<Option<Organization>> UpdateAsync(Organization o);
        #endregion

        #region  UserManagement
        /// <returns>A list of UserOrganizations with populated User NPs who have access to the Organization.</returns>
        Task<List<UserOrganization>> GetUsersOfOrgAsync(Guid orgId);

        /// <returns>A list of UserOrganizations with populated User NPs awaiting access approval.</returns>
        Task<List<UserOrganization>> GetUsersOfOrgAwaitingApprovalAsync(Guid orgId);

        /// <returns>An Option containing a UserOrganization with populated User NP if found. Only returns approved.</returns>
        Task<Option<UserOrganization>> GetUserOfOrgByIdsAsync(Guid orgId, Guid userId);

        /// <summary>
        /// Updates the roles and tenant-specific profile information of the UserOrganization. Assumes
        /// User, UserOrganizationRoles, and Roles NPs are populated, and only saves changes to the 
        /// UserOrganization and UserOrganizationRoles
        /// </summary>
        Task UpdateUserOfOrgAsync(UserOrganization uo);
        #endregion

        #region RoleManagement
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
        Task UpdateRoleOfOrgAsync(Guid orgId, Role role);

        /// <summary>
        /// Attempts to delete the role. This will fail if any user has
        /// this role as their only role, and will not delete global roles.
        /// </summary>
        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> DeleteRoleOfOrgAsync(Role role);
        #endregion

        #region InvitationManagement
        /// <summary>
        /// Returns a code that can be reused to grant a user access to the organization.
        /// </summary>
        Task<string> CreatePermanentInvitationLinkAsync(Organization org);

        /// <summary>
        /// Uses the specified permanent invitation code to attempt to grant the user access to the organization.
        /// </summary>
        Task<InviteResult> UsePermanentInvitationAsync(User user, string link);
        #endregion

        #region PermissionAndAccessChecks
        /// <returns>Whether the User has access to the Organization, or false if either doesn't exist.</returns>
        Task<bool> UserHasAccessAsync(Guid userId, Guid orgId);

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
        #endregion
    }
}