using CoreMultiTenancy.Identity.Tenancy;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Interacts with requests to provide tenant information.
    /// </summary>
    public interface ITenantHook
    {
        Tenant GetRequestTenant();
    }
}