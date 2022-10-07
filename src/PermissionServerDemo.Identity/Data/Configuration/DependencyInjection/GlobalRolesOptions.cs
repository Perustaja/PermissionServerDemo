using PermissionServerDemo.Identity.Entities;

namespace PermissionServerDemo.Identity.Data.Configuration.DependencyInjection
{
    public class GlobalRolesOptions
    {
        private List<GlobalRoleBuilder> _builders = new List<GlobalRoleBuilder>();
        public List<Role> Roles => _builders.Select(b => b.BuildRole()).ToList();
        public List<RolePermission> RolePermissions => _builders.SelectMany(b => b.BuildPermissions()).ToList();
        public void AddGlobalRole(Action<GlobalRoleBuilder> ba)
        {
            var b = new GlobalRoleBuilder();
            ba?.Invoke(b);
            _builders.Add(b);
        }
    }
}