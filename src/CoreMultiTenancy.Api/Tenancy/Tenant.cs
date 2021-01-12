namespace CoreMultiTenancy.Api.Tenancy
{
    public struct Tenant
    {
        public Tenant(string id)
        {
            Id = id;
            ConnectionString = id; // Possibly perform some transformation or remove certain characters?
        }
        public string Id;
        public string ConnectionString;
    }
}