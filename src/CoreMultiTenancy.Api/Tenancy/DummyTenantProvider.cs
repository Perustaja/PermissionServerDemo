using System;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Tenancy
{
    /// <summary>
    /// Provides a way to handle design-time migrations while still requiring tenant information for each
    /// request during main application execution.
    /// </summary>
    public class DummyTenantProvider : ITenantProvider
    {
        private readonly string _dummyId;

        public DummyTenantProvider(IConfiguration config)
        {
            _dummyId = config["DummyTenantId"] ?? throw new ArgumentNullException("Unable to source dummy tenant id from config.");
        }

        public Tenant GetCurrentRequestTenant() => new Tenant(_dummyId);
    }
}