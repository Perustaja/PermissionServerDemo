namespace CoreMultiTenancy.Api.Tenancy
{
    /// <summary>
    /// Represents a DbContext that provides a unique, per-tenant identifier used for caching the model.
    /// </summary>
    public interface ITenantedDbContext
    {
        string TenantModelCacheKey { get; }
    }
}