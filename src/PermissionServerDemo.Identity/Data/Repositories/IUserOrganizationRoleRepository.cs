using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Interfaces;

namespace PermissionServerDemo.Identity.Data.Repositories
{
    /// <summary>
    /// Handles User Role management for Organizations. This updates and deletes Roles of User's within an
    /// Organization, which are loaded as related data from UserOrganizationRepository.
    /// </summary>
    public interface IUserOrganizationRoleRepository : IRepository
    {
        /// <returns>The UserOrganizationRole entity being tracked after add.</returns>
        UserOrganizationRole Add(UserOrganizationRole uor);
        void Update(List<UserOrganizationRole> uors);
        void Delete(UserOrganizationRole uor);
        /// <returns>Whether a User has this Role as their only Role, and deleting it would leave them without one.</returns>
        Task<bool> RoleIsOnlyRoleForAnyUserAsync(Role role);
        /// <returns>Either a list representing the roles the user has within the organization, or empty list</returns>
        Task<List<UserOrganizationRole>> GetUsersRolesAsync(Guid userId, Guid orgId);
    }
}