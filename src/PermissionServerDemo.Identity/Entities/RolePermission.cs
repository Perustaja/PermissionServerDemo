using PermissionServer.Common;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Identity.Entities
{
    /// <summary>
    /// Join table representing permissions roles have.
    /// </summary>
    public class RolePermission
    {
        public Guid RoleId { get; private set; }
        public string PermissionId { get; private set; }
        public Role Role { get; set; }
        public Permission<PermissionEnum, PermissionCategoryEnum> Permission { get; set; }
        public RolePermission() { }
        public RolePermission(Guid roleId, PermissionEnum permId)
        {
            RoleId = roleId;
            PermissionId = permId.ToString();
        }
    }
}