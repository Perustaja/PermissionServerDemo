using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
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
        /// Returns all roles that the accompanied organization has, including global roles.
        /// </summary>
        Task<List<Role>> GetRolesAsync(Guid orgId);

        /// <summary>
        /// Adds a role to be used by the specified organization.
        /// </summary>
        Task AddRoleAsync(Guid orgId, string desc, params PermissionEnum[] perms);
        
        /// <summary>
        /// Returns a list of all organizations that the specified user has access to, or null.
        /// </summary>
        Task<List<Organization>> GetUsersOrgsAsync(Guid userId);

        /// <summary>
        /// Returns a list of permissions that the specified user has in the scope of the given organization.
        /// </summary>
        Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns a list of roles that the user has in the scope of the given organization.
        /// </summary>
        Task<List<Role>> GetUsersRolesAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns a code that can be reused to grant a user access to the organization.
        /// </summary>
        Task<string> CreatePermanentInvitationLinkAsync(Guid orgId);

        /// <summary>
        /// Uses the specified permanent invitation code to attempt to grant the user access to the organization.
        /// </summary>
        Task<InviteResult> UsePermanentInvitationAsync(User user, string link);

        /// <summary>
        /// Attempts to grant the user access to the organization.
        /// </summary>
        Task<AccessModifiedResult> GrantAccessAsync(User user, Organization org);

        /// <summary>
        /// Attempts to revoke the corresponding user's access to the corresponding organization based on
        /// id values. Returns false if any Exception occurs.
        /// </summary>
        Task<bool> RevokeAccessAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the corresponding user has access to the corresponding organization.
        /// </summary>
        Task<bool> UserHasAccessAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the user has permission within the scope of the organization.
        /// </summary>
        Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum p);
    }
}