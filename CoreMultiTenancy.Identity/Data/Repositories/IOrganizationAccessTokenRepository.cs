using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    interface IOrganizationAccessTokenRepository
    {
        Task AddTokenAsync(OrganizationAccessToken token);
        Task DeleteTokenAsync(OrganizationAccessToken token);
        /// <summary>
        /// Returns the OrganizationAccessToken based on the token value, or null.
        /// </summary>
        Task<OrganizationAccessToken> GetTokenByValueAsync(string value);
    }
}