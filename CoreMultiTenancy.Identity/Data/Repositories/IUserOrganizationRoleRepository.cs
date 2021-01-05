using System.Collections.Generic;
using CoreMultiTenancy.Identity.Interfaces;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Handles User Role management for Organizations. This updates and deletes Roles of User's within an
    /// Organization, which are loaded as related data from UserOrganizationRepository.
    /// </summary>
    public interface IUserOrganizationRoleRepository : IRepository
    {
        /// <returns>The UserOrganizationRole entity being tracked after add.</returns>
        UserOrganizationRole Add(UserOrganizationRole uor);

        void UpdateBulk(List<UserOrganizationRole> uors);

        void DeleteBulk(List<UserOrganizationRole> uors); 
    }
}