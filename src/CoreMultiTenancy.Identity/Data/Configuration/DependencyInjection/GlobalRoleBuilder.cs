using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Data.Configuration.DependencyInjection
{
    /// <summary>
    /// Configures a single global role to be seeded into the database. Start WithBaseRole() then add 
    /// permissions as desired.
    /// </summary>
    public class GlobalRoleBuilder
    {
        private Role _role;
        public Role Build() => _role;
        public GlobalRoleBuilder WithBaseRole(Guid id, string name, string desc)
        {
            var r = Role.SeededGlobalRole(id, name, desc);
            _role = r;
            return this;
        }

        public GlobalRoleBuilder AsDefaultAdminRole()
        {
            ensureBaseRoleCreated();
            _role.SetAsGlobalAdminRole();
            return this;
        }

        public GlobalRoleBuilder AsDefaultNewUserRole()
        {
            ensureBaseRoleCreated();
            _role.SetAsGlobalDefaultNewUserRole();
            return this;
        }

        public GlobalRoleBuilder GrantAllPermissions()
        {
            ensureBaseRoleCreated();
            // add permissions to be saved into the db later
            foreach (PermissionEnum p in Enum.GetValues(typeof(PermissionEnum)))
                _role.RolePermissions.Add(new RolePermission(_role.Id, p));
            return this;
        }

        private void ensureBaseRoleCreated()
        {
            if (_role == null)
                throw new Exception("Attempted to configure a global role without specifying a base role. Ensure WithBaseRole() is called before further configuration.");
        }
    }
}