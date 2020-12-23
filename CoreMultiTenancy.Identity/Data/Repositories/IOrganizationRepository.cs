using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IOrganizationRepository
    {
        /// <summary>
        /// Returns an Organization based on the given id, or null.
        /// </summary>
        Task<Organization> GetByIdAsync(Guid id);
        /// <summary>
        /// Returns all valid Organizations that a User has access to.
        /// </summary>
        Task<List<Organization>> GetUsersOrgsById(Guid userId);
    }
}