using CoreMultiTenancy.Api.Interfaces;

namespace CoreMultiTenancy.Api.Tenancy
{
    /// <summary>
    /// Provides a way to handle design-time migrations or times where a specific tenant needs
    /// to be injected. This should not be registered in DI, it should be passed into ActivatorUtilities.
    /// </summary>
    public class ManualTenantProvider : ITenantProvider
    {
        private readonly string _tenantId;

        public ManualTenantProvider(string tenantId)
        {
            _tenantId = tenantId;
        }

        public Tenant GetCurrentRequestTenant() => new Tenant(_tenantId);
    }
}