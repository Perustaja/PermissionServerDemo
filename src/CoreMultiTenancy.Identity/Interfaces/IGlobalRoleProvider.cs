using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IGlobalRoleProvider
    {
        public List<Role> GetGlobalRoles();
        public List<RolePermission> GetGlobalRolePermissions();
    }
}