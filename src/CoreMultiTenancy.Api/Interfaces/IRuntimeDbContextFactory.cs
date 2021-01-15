using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Api.Interfaces
{
    /// <summary>
    /// Initializes contexts and handles creation of newly added tenants.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IRuntimeDbContextFactory<TContext> where TContext : DbContext
    {
        /// <summary>
        /// Returns the Context based on the current request, if the backing database does not currently 
        /// exist, it creates it, applies migrations, and does any necessary seeding before returning it.
        /// </summary>
        /// <returns>A tenant-specific context.</returns>
        TContext CreateContext();
    }
}