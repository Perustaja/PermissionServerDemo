using System;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Join table representing permissions roles have.
    /// </summary>
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public string PermissionId { get; private set; }
        public Role Role { get; set; }
        public Permission Permission { get; set; }
        public RolePermission() { }
        
        public RolePermission(Guid roleId, string permId)
        {
            RoleId = roleId;
            PermissionId = permId;
        }
    }
}