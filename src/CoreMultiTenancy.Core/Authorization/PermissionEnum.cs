using CoreMultiTenancy.Core.Attributes;

namespace CoreMultiTenancy.Core.Authorization
{
    /// <summary>
    /// Main application permissions represented as an enum. Changing underlying byte value WILL introduce
    /// breaking changes to database. Each value must have a PermissionSeedData attribute.
    /// Use [Obsolete] if one is deprecated.
    /// </summary>
    public enum PermissionEnum : byte
    {
        [PermissionSeedData("Default", PermissionCategoryEnum.Default, VisibleToUser = false)]
        Default = 0,
        [PermissionSeedData("All", PermissionCategoryEnum.Default, VisibleToUser = false)]
        All = 1,
        [PermissionSeedData("Create Aircraft", PermissionCategoryEnum.Aircraft, "Users with this permission can create new aircraft within the tenant.")]
        AircraftCreate = 2,
    }
}