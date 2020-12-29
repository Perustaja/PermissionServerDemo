using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Repository for dealing with roles and permissions including join tables.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Returns all roles of the given organization.
        /// </summary>
        /// <returns></returns>
        Task<List<Role>> GetRolesAsync(Guid orgId);

        /// <summary>
        /// Creates a new role with the permissions within the scope of the associated organization.
        /// </summary>
        Task<bool> AddRoleAsync(Guid orgId, string desc, params PermissionEnum[] perms);

        /// <summary>
        /// Deletes the role associated with the given id if found. Will not delete global roles.
        /// </summary>
        Task<bool> DeleteRoleAsync(Guid roleId);

        /// <summary>
        /// Adds the role associated with the id to the user if both exist.
        /// </summary>
        Task<bool> AddRoleToUserAsync(Guid roleId, Guid userId);

        /// <summary>
        /// Removes the role associated with the id from the user if both exist.
        /// </summary>
        Task<bool> RemoveRoleFromUserAsync(Guid roleId, Guid userId);

        /// <summary>
        /// Returns a list of permissions that the user has based on the user and organization ids.
        /// If user does not have access or has no permissions, returns empty list.
        /// </summary>
        Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns a list of roles that the user has based on the user and organization ids.
        /// If user does not have access or has no roles, returns empty list.
        /// </summary>
        Task<List<Role>> GetUsersRolesAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the given user has the permission within the scope of the organization.
        /// </summary>
        Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId);
    }
}