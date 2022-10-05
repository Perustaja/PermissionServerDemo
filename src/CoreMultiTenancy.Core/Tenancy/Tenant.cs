using System;

namespace CoreMultiTenancy.Core.Tenancy
{
    public class Tenant
    {
        public Tenant(Guid id) => Id = id;
        public readonly Guid Id;
    }
}