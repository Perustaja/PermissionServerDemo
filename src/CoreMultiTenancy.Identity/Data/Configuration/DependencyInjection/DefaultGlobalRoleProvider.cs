using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;

namespace CoreMultiTenancy.Identity.Data.Configuration.DependencyInjection
{
    public class DefaultGlobalRoleProvider : IGlobalRoleProvider
    {
        private readonly GlobalRolesOptions _options;

        public DefaultGlobalRoleProvider(GlobalRolesOptions options)
        {   
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public List<Role> GetGlobalRoles() => _options.Roles;
    }
}