using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Handles User management for Organizations.
    /// </summary>
    public interface IUserOrganizationRepository
    {
        /// <returns>All UserOrganization records with populated User NPs.</returns>
        Task<List<UserOrganization>> GetAllByOrgId(Guid orgId);

        /// <returns>All UserOrganization records awaiting access with populated User NPs.</returns>
        Task<List<UserOrganization>> GetAwaitingAccessByOrgId(Guid orgId);

        /// <returns>An Option containing a UserOrganization with populated User NP if found.</returns>
        Task<Option<UserOrganization>> GetByIdsAsync(Guid orgId, Guid userId);

        /// <summary>
        /// Adds the UserOrganization.
        /// </summary>
        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> AddAsync(UserOrganization uo);

        /// <summary>
        /// Attempts to update the UserOrganization entity.
        /// </summary>
        Task UpdateAsync(UserOrganization uo);

        /// <returns>Whether the Organization has a record of the User, even if awaiting approval or blacklisted.</returns>
        Task<bool> ExistsAsync(Guid userId, Guid orgId);
        
        /// <returns>Whether the User has active access to the Organization.</returns>
        Task<bool> ExistsWithAccessAsync(Guid userId, Guid orgId);
    }
}