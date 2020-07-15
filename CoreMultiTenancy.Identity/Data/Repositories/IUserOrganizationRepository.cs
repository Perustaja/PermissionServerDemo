using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IUserOrganizationRepository
    {
        Task AddAsync(UserOrganization uo);
        Task DeleteAsync(UserOrganization uo);
        
        /// <summary>
        /// Returns whether the User associated with the first id has access to the Organization
        /// associated with the second id.
        /// </summary>
        Task<bool> UserHasAccess(Guid userId, Guid orgId);
    }
}