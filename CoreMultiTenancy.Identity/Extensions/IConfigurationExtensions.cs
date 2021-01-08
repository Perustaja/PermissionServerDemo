using System;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Verifies current configuration contains a guid value associated with the key "DefaultRoleId" 
        /// and returns it.
        /// </summary>
        /// <exception cref="System.Exception">If no configuration found</exception>
        /// <returns>The default role id as a Guid.</returns>
        public static Guid GetDefaultRoleId(this IConfiguration configuration)
        {
            if (Guid.TryParse(configuration["DefaultRoleId"], out Guid res))
                return res;
            throw new Exception("Location of DefaultRoleId or parsing to Guid failed.");
        }
    }
}