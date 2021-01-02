using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IOrganizationRepository
    {
        /// <returns>All valid Organizations that a User has access to.</returns>
        Task<List<Organization>> GetUsersOrgsById(Guid userId);

        /// <returns>Whether an Organization with the given id exists.</returns>
        Task<bool> ExistsByIdAsync(Guid id);

        /// <returns>An Option with the Organization if found.</returns>
        Task<Option<Organization>> GetByIdAsync(Guid id);

        /// <returns>An Option containing the Organization if successful.</returns>
        Task<Option<Organization>> AddAsync(Organization o);

        /// <returns>An Option containing the Organization if successful.</returns>
        Task<Option<Organization>> UpdateAsync(Organization o);
    }
}