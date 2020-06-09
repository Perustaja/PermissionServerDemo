using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Tenancy;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Serves tenants from storage.
    /// </summary>
    public interface ITenantSource
    {
        Task<List<Tenant>> GetTenantOrDefaultById(Guid id);
    }
}