namespace CoreMultiTenancy.Api.Tenancy
{
    public interface ITenantProvider
    {
        /// <exception cref="Tenancy.TenantNotFoundException">If Tenant is not found for current request.</exception>
        /// <returns>A Tenant struct that is not null.</returns>
        Tenant GetCurrentRequestTenant();
    }
}