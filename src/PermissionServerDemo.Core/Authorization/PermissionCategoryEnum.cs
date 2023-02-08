using PermissionServer.Common;

namespace PermissionServerDemo.Core.Authorization
{
    public enum PermissionCategoryEnum : byte
    {
        [CategoryData("Aircraft")]
        Aircraft,
        [CategoryData("Roles")]
        Roles,
        [CategoryData("Users")]
        Users,
    }
}