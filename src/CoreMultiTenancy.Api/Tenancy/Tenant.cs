namespace CoreMultiTenancy.Api.Tenancy
{
    public struct Tenant
    {
        public Tenant(string id) => Id = id;
        public readonly string Id;
    }
}