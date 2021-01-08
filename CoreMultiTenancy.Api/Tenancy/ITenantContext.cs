using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Api.Tenancy
{
    public interface ITenantContext
    {
        /// <returns>
        /// An Option containing the identifier for the tenant associated with the current request if
        /// it exists.
        /// </returns>
        Option<string> GetTenantIdentifier();
    }
}