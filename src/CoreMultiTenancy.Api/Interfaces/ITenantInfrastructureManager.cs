using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Api.Interfaces
{
    /// <summary>
    /// Handles tenant infrastructure creation and deletion. Tenant information is sourced
    /// from the current HttpContext based on the protocol.
    /// </summary>
    public interface ITenantInfrastructureManager<TContext> where TContext : DbContext, ITenantedDbContext
    {
        /// <summary>
        /// Creates necessary infrastucture for a newly created tenant.
        /// </summary>
        /// <returns>
        /// Whether the outcome of the infrastructure creation was successful. This defaults
        /// to true if the tenant's database already exists.
        /// </returns>
        Task<bool> InitializeTenantAsync(string tenantId);
        /// <summary>
        /// Removes all infrastructure of a tenant permanently.
        /// </summary>
        /// <returns>
        /// Whether the outcome of the infrastructure deletion was successful. This defaults
        /// to true if the tenant's database doesn't exist.
        /// </returns>
        Task<bool> DeleteTenantAsync(string tenantId);
    }
}