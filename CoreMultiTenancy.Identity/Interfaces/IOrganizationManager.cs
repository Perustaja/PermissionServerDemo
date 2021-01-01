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
    /// Handles role management, tenant access, and tenant management.
    /// </summary>
    public interface IOrganizationManager
    {
        /// <summary>
        /// Returns an organization associated with the given id.
        /// </summary>
        Task<Option<Organization>> GetByIdAsync(Guid orgId);

        /// <summary>
        /// Returns all organizations.
        /// </summary>
        Task<List<Organization>> GetAllAsync();

        /// <summary>
        /// Updates the given tenant.
        /// </summary>
        /// <returns>The updated organization.</returns>
        Task<Option<Organization>> UpdateAsync(Organization org);

        /// <summary>
        /// Returns the role with its permissions navigation property populated.
        /// </summary>
        Task<Option<Role>> GetRoleWithPermsByIdAsync(Guid orgId);

        /// <summary>
        /// Returns all roles that the accompanied organization has, including global roles.
        /// If list is empty, it can be assumed that the organization does not exist.
        /// </summary>
        Task<List<Role>> GetAllRolesAsync(Guid orgId);

        /// <summary>
        /// Adds a role to be used by the specified organization.
        /// </summary>
        /// <returns>The added role.</returns>
        Task<Option<Role>> AddRoleAsync(Role role);

        /// <summary>
        /// Adds a role to be used by the specified organization.
        /// </summary>
        /// <returns>The updated role.</returns>
        Task<Option<Role>> UpdateRoleAsync(Role role);

        /// <summary>
        /// Attempts to delete the role, removing all traces of it. This will fail if any user has
        /// this role as their only role or if the role is global.
        /// </summary>
        Task<Option<Error>> DeleteRoleAsync(Role role);

        /// <summary>
        /// Returns a list of all organizations that the specified user has access to, or null.
        /// </summary>
        Task<List<Organization>> GetUsersOrgsAsync(Guid userId);

        /// <summary>
        /// Returns a list of users with their roles.
        /// </summary>
        Task<List<User>> GetUsersWithRolesAsync(Guid userId);

        /// <summary>
        /// Returns all users awaiting approval.
        /// </summary>
        Task<List<User>> GetUsersAwaitingApprovalAsync(Guid orgId);

        /// <summary>
        /// Returns all users awaiting approval.
        /// </summary>
        Task<List<User>> GetBlacklistedUsersAsync(Guid orgId);

        /// <summary>
        /// Returns a code that can be reused to grant a user access to the organization.
        /// </summary>
        Task<string> CreatePermanentInvitationLinkAsync(Organization org);

        /// <summary>
        /// Uses the specified permanent invitation code to attempt to grant the user access to the organization.
        /// </summary>
        Task<InviteResult> UsePermanentInvitationAsync(User user, string link);

        /// <summary>
        /// Attempts to grant the user(s) access to the organization. This is for users who have used
        /// an invitation, but need to be manually verified.
        /// </summary>
        Task<AccessModifiedResult> GrantAccessAsync(Organization org, params User[] users);

        /// <summary>
        /// Attempts to revoke the corresponding user's access to the corresponding organization based on
        /// id values.
        /// </summary>
        Task<Option<Error>> RevokeAccessAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the corresponding user has access to the corresponding organization.
        /// Returns false if either doesn't exist.
        /// </summary>
        Task<bool> UserHasAccessAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the user has permission within the scope of the organization.
        /// Returns false if either doesn't exist or the user does not have access.
        /// </summary>
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params PermissionEnum[] perms);
    }
}