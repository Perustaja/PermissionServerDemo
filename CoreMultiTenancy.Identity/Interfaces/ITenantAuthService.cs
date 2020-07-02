using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Contains methods for verifying user and tenant information.
    /// </summary>
    public interface ITenantAuthService<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Ensures that the user associated with the subject's id has access to its selected organization.
        /// Returns false if User's access was revoked.
        /// <param name="userId">The User's id.</param>
        /// </summary>
        Task<bool> ValidateUserAsync(TKey subId);
        /// <summary>
        /// Should be called whenever an Organization revokes access for one of its Users. Ensures that
        /// proper entries are recorded and the authorization system reacts if necessary.
        /// <param name="userId">The id of the User whose access was revoked.</param>
        /// <param name="orgId">The id of the Organization who is revoking access.</param>
        /// </summary>
        Task UserAccessRevokedAsync(TKey userId, TKey orgId);
    }
}