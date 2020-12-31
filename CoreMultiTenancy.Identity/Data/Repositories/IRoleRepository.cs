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
        /// <summary>
        /// Returns all roles of the given organization without related permissions.
        /// </summary>
        Task<List<Role>> GetAllByOrgIdAsync(Guid orgId);
        
        /// <summary>
        /// Returns a single role with related permissions.
        /// </summary>
        Task<Option<Role>> GetByIdAsync(Guid roleId);

        /// <summary>
        /// Creates a new role with the permissions within the scope of the associated organization.
        /// </summary>
        /// <returns>The added role.</returns>
        Task<Option<Role>> AddRoleAsync(Role role);

        /// <summary>
        /// Deletes the role associated with the given id if found. Will not delete global roles.
        /// </summary>
        Task<Option<Error>> DeleteRoleAsync(Role role);

        Task<Option<Error>> AddUserOrganizationRole(UserOrganizationRole uor);

        Task<Option<Error>> RemoveUserOrganizationRole(UserOrganizationRole uor);

        /// <summary>
        /// Returns a list of permissions that the user has based on the user and organization ids.
        /// </summary>
        Task<List<Permission>> GetUsersPermissionsAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns a list of roles that the user has based on the user and organization ids.
        /// </summary>
        Task<List<Role>> GetUsersRolesAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the given user has the permission within the scope of the organization.
        /// </summary>
        Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm);
    }
}