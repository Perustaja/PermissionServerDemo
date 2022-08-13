using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IOrganizationRepository : IRepository
    {
        /// <returns>All valid Organizations that a User has access to.</returns>
        Task<List<Organization>> GetUsersOrgsByIdAsync(Guid userId);

        /// <returns>Whether an Organization with the given id exists.</returns>
        Task<bool> ExistsByIdAsync(Guid id);

        /// <returns>An Option with the Organization if found.</returns>
        Task<Option<Organization>> GetByIdAsync(Guid id);

        /// <returns>The Organization entity being tracked upon add.</returns>
        Organization Add(Organization o);

        /// <returns>The Organization entity being tracked upon update.</returns>
        Organization Update(Organization o);

        void Delete(Organization o);
    }
}