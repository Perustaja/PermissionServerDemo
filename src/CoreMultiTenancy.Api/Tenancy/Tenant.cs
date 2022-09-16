namespace CoreMultiTenancy.Api.Tenancy
{
    public class Tenant
    {
        public Tenant(Guid id) => Id = id;
        public readonly Guid Id;
    }
}