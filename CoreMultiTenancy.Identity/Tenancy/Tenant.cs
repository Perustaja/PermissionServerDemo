using System;

namespace CoreMultiTenancy.Identity.Tenancy
{
    public abstract class Tenant
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string ConnectionString { get; private set; }
    }
}