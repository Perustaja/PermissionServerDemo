
using PermissionServer;

namespace PermissionServerDemo.Api.Data
{
    /// <summary>
    /// Provides a way to handle design-time migrations.
    /// This should not be registered in DI, it should be passed into ActivatorUtilities.
    /// </summary>
    public class ManualTenantProvider : ITenantProvider
    {
        private readonly Guid _tenantId;
        public ManualTenantProvider(Guid tenantId)
        {
            _tenantId = tenantId;
        }

        public Guid GetCurrentRequestTenant() => _tenantId;
    }
}