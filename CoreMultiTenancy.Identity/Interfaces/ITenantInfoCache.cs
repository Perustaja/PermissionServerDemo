using System;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Interface for a cache that provides tenant information for middleware.
    /// <typeparam name="TTenant">The model representing your tenant.</typeparam>
    /// <typeparam name="TKey">The identifier of your tenant.</typeparam>
    /// </summary>
    public interface ITenantInfoCache<TTenant, TKey>
    where TTenant : class 
    where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Retrives a single Organization based on the identifier or null.
        /// </summary>
        Organization GetOrganization(TKey id);
        /// <summary>
        /// Retrives a single UserOrganization based on the user and tenant ids or null.
        /// </summary>
        UserOrganization GetUserOrganization(TKey userId, TKey tenantId);
    }
}