using System.ComponentModel.DataAnnotations;
using PermissionServerDemo.Core.Attributes;

namespace PermissionServerDemo.Core.Authorization
{
    /// <summary>
    /// Main application permission categories represented as an enum. Changing underlying string 
    /// value WILL introduce breaking changes to database. Each value must have a DisplayAttribute 
    /// setting its Name value. Use [Obsolete] if one is deprecated.
    /// </summary>
    public enum PermissionCategoryEnum : byte
    {
        [PermissionCategorySeedData("Aircraft")]
        Aircraft,
        [PermissionCategorySeedData("Roles")]
        Roles,
        [PermissionCategorySeedData("Users")]
        Users,
    }
}