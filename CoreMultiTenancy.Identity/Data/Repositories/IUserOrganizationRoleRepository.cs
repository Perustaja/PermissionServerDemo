using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Handles User Role management for Organizations. This updates and deletes Roles of User's within an
    /// Organization, which are loaded as related data from UserOrganizationRepository.
    /// </summary>
    public interface IUserOrganizationRoleRepository
    {
        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> AddAsync(UserOrganizationRole uor);

        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> UpdateBulkAsync(List<UserOrganizationRole> uors);

        /// <returns>An Option containing an Error on failure.</returns>
        Task<Option<Error>> DeleteBulkAsync(List<UserOrganizationRole> uors); 
    }
}