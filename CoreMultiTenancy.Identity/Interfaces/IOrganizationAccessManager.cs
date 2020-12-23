using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IOrganizationAccessManager
    {
        /// <summary>
        /// Attempts to grant the User access to the Organization and returns an AccessModifiedResult
        /// with details of the outcome.
        /// </summary>
        Task<AccessModifiedResult> GrantAccessAsync(User user, Organization org);
        /// <summary>
        /// Attempts to revoke the corresponding User's access to the corresponding Organization based on
        /// id values. Returns false if any Exception occurs.
        /// </summary>
        Task<bool> RevokeAccessAsync(Guid userId, Guid orgId);
        /// <summary>
        /// Returns whether the corresponding User has access to the corresponding Organization.
        /// </summary>
        Task<bool> UserHasAccessAsync(Guid userId, Guid orgId);
    }
}