using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IAccessRevokedEventRepository
    {
        /// <summary>
        /// Returns an AccessRevokedEvent object if a record exists under the given User's id, or null.
        /// Throws an exception if multiple entries exist under this id.
        /// </summary>
        Task<AccessRevokedEvent> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid userId);
        Task AddAsync(Guid userId, Guid OrgId);
    }
}