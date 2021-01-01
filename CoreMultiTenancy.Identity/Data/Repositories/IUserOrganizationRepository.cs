using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IUserOrganizationRepository
    {
        Task<Option<Error>> AddAsync(UserOrganization uo);
        Task DeleteAsync(UserOrganization uo);

        /// <summary>
        /// Returns a single UserOrganization based on the ids.
        /// </summary>
        Task<Option<UserOrganization>> GetByIdsAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether a record exists, even if it is awaiting approval or blacklisted.
        /// </summary>
        Task<bool> ExistsAsync(Guid userId, Guid orgId);
        
        /// <summary>
        /// Returns whether a record exists that is not awaiting approval nor blacklisted.
        /// </summary>
        Task<bool> ExistsWithAccessAsync(Guid userId, Guid orgId);
    }
}