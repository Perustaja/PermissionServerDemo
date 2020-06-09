using System;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IMultiTenantDbContext
    {
        /// <summary>
        /// The id of the given request's tenant.
        /// </summary>
        Guid TenantId { get; }   
    }
}