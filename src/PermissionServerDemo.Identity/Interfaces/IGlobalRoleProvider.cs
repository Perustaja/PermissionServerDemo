using PermissionServerDemo.Identity.Entities;

namespace PermissionServerDemo.Identity.Interfaces
{
    public interface IGlobalRoleProvider
    {
        public List<Role> GetGlobalRoles();
        public List<RolePermission> GetGlobalRolePermissions();
    }
}