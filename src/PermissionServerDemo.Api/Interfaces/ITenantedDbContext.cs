namespace PermissionServerDemo.Api.Interfaces
{
    /// <summary>
    /// Represents a DbContext that provides a unique, per-tenant identifier used for caching the model.
    /// </summary>
    public interface ITenantedDbContext
    {
        string TenantId { get; }
        string TenantModelCacheKey { get; }
    }
}