using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.Extensions.Options;

namespace CoreMultiTenancy.Identity.Data.Configuration.DependencyInjection
{
    public class DefaultGlobalRoleProvider : IGlobalRoleProvider
    {
        private readonly GlobalRolesOptions _options;
        public DefaultGlobalRoleProvider(IOptions<GlobalRolesOptions> options)
        {   
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public List<Role> GetGlobalRoles() => _options.Roles;
        public List<RolePermission> GetGlobalRolePermissions() => _options.RolePermissions;
    }
}