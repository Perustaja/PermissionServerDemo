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
        /// Returns a single UserOrganization based on the ids, or null.
        /// </summary>
        Task<UserOrganization> GetByIdsAsync(Guid userId, Guid orgId);
        Task<bool> ExistsAsync(Guid userId, Guid orgId);
    }
}