using System;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Verifies current configuration contains a guid value associated with the key "DefaultAdminRoleId" 
        /// and returns it.
        /// </summary>
        /// <exception cref="System.Exception">If no configuration found</exception>
        /// <returns>The default admin role with all permissions id.</returns>
        public static Guid GetDefaultAdminRoleId(this IConfiguration configuration)
        {
            if (Guid.TryParse(configuration["DefaultAdminRoleId"], out Guid res))
                return res;
            throw new Exception("Location of DefaultAdminRoleId or parsing to Guid failed.");
        }

        /// <summary>
        /// Verifies current configuration contains a guid value associated with the key "DefaultLowestUserRoleId" 
        /// and returns it.
        /// </summary>
        /// <exception cref="System.Exception">If no configuration found</exception>
        /// <returns>The default new user role id.</returns>
        public static Guid GetDefaultLowestUserRoleId(this IConfiguration configuration)
        {
            if (Guid.TryParse(configuration["DefaultLowestUserRoleId"], out Guid res))
                return res;
            throw new Exception("Location of DefaultLowestUserRoleId or parsing to Guid failed.");
        }
    }
}