using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    /// <summary>
    /// Handles User management for Organizations.
    /// </summary>
    public interface IUserOrganizationRepository : IRepository
    {
        /// <returns>All UserOrganization records with populated User, UserOrgRole, and Role NPs.</returns>
        Task<List<UserOrganization>> GetAllByOrgId(Guid orgId);

        /// <returns>All UserOrganization records awaiting access with populated User, UserOrgRole, and Role NPs.</returns>
        Task<List<UserOrganization>> GetAwaitingAccessByOrgId(Guid orgId);

        /// <returns>An Option containing a UserOrganization with populated User, UserOrgRole, and Role NPs if found.</returns>
        Task<Option<UserOrganization>> GetByIdsAsync(Guid orgId, Guid userId);

        /// <returns>The UserOrganization entity being tracked on add.</returns>
        UserOrganization Add(UserOrganization uo);

        /// <returns>The UserOrganization entity being tracked upon add.</returns>
        UserOrganization Update(UserOrganization uo);

        /// <returns>Whether the Organization has a record of the User, even if awaiting approval or blacklisted.</returns>
        Task<bool> ExistsAsync(Guid userId, Guid orgId);

        /// <returns>Whether the User has active access to the Organization.</returns>
        Task<bool> ExistsWithAccessAsync(Guid userId, Guid orgId);
    }
}