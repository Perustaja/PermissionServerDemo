using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Retrieves a formatted connection string based on a template string provided by configuration.
        /// </summary>
        /// <param name="tid">The id of current request's tenant.</param>
        /// <returns>A formatted connection string ready to be used for database connection.</returns>
        public static string GetTenantedConnectionString(this IConfiguration config, string tid)
            => config.GetConnectionString("TemplateString").Replace("{dbname}", tid);
    }
}