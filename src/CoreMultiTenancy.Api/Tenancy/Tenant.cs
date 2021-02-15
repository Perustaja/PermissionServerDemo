namespace CoreMultiTenancy.Api.Tenancy
{
    public class Tenant
    {
        public Tenant(string id) => Id = id;
        public readonly string Id;
    }
}