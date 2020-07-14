using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Interface for allowing Organization access to new/existing Users.
    /// </summary>
    interface IOrganizationAccessTokenService
    {
        /// <summary>
        /// Sends an email that offers access to an Organization via a one-time link.
        /// <param name="email">The destination email.</param>
        /// <param name="orgId">The id of the Organization to grant access to.</param>
        /// </summary>
        Task SendInvitation(string email, Guid orgId);

        /// <summary>
        /// Expends the token if it was created for the given User, granting it access to the
        /// Organization.
        /// <param name="value">The token value.</param>
        /// <param name="user">The User who is activating this token.</param>
        /// </summary>
        Task UseToken(string value, User user);

        /// <summary>
        /// Checks if the given token was issued for this User and is valid and non-expired.
        /// <param name="value">The token value.</param>
        /// <param name="user">The User who is activating this token.</param>
        /// </summary>
        Task<bool> IsTokenValid(string value, User user);
    }
}