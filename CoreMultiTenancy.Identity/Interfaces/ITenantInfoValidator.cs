using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Contains methods for verifying user and tenant information.
    /// </summary>
    public interface ITenantInfoValidator<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Ensures that the user associated with the given id has access to an active organization.
        /// <param name="userId">The User's id.</param>
        /// <param name="selectedOrg">The desired Organization to scope to.</param>
        /// </summary>
        TenantValidationResult ValidateSelectedOrganization(TKey userId, TKey selectedOrg);
    }
}