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
    /// This repository is made to handle instantiated objects and be as simple as possible, while
    /// IOrganizationManager coordinates, instantiates, and checks for existing data beforehand to return
    /// more precise errors.
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
        /// Updates the specified role.
        /// </summary>
        /// <returns>The updated role.</returns>
        Task<Option<Role>> UpdateRoleAsync(Role role);

        /// <summary>
        /// Deletes the role associated with the given id if found. Will not delete global roles.
        /// </summary>
        Task DeleteRoleAsync(Role role);

        /// <summary>
        /// Returns a list of roles that the user has based on the user and organization ids.
        /// This list of UserOrganizationRoles represents the roles a user has for the given org.
        /// </summary>
        Task<List<Role>> GetUserOrganizationRolesByIdsAsync(Guid userId, Guid orgId);

        Task<Option<Error>> AddUserOrganizationRoleAsync(UserOrganizationRole uor);

        Task DeleteUserOrganizationRoleAsync(UserOrganizationRole uor);

        /// <summary>
        /// Returns a readonly list of permissions that the user has based on the user and organization ids.
        /// If not meant to be displayed to user as a list, prefer to use UserHasPermissionAsync if checking
        /// for authorization purposes.
        /// </summary>
        Task<IReadOnlyList<Permission>> GetUsersPermissionsAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the given user has the permission within the scope of the organization.
        /// Optimized for authorization checks.
        /// </summary>
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params PermissionEnum[] perm);
    }
}