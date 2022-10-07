namespace PermissionServerDemo.Core.Tenancy
{
    /// <summary>
    /// Provides access to the information of the tenant for a request.
    /// </summary>
    public interface ITenantProvider
    {
        /// <returns>A non-null Tenant for the request.</returns>
        /// <exception cref="Tenancy.TenantNotFoundException">If Tenant is not found for current request.</exception>
        Tenant GetCurrentRequestTenant();
    }
}