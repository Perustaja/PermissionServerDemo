using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IOrganizationRepository
    {
        /// <summary>
        /// Returns an Organization based on the given id, or null.
        /// </summary>
        Task<Organization> GetByIdAsync(Guid id);
    }
}