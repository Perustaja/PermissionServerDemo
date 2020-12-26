using CoreMultiTenancy.Identity.Attributes;

namespace CoreMultiTenancy.Identity.Authorization
{
    /// <summary>
    /// Main application permissions represented as an enum. Changing underlying byte value WILL introduce
    /// breaking changes to database. Each value must have a PermissionSeedData attribute.
    /// Use [Obsolete] if one is deprecated.
    /// </summary>
    public enum PermissionEnum : byte
    {
        [PermissionSeedData("Default", PermissionCategoryEnum.Default)]
        Default = 0,

        [PermissionSeedData("Create Aircraft", PermissionCategoryEnum.Resource)]
        AircraftCreate = 1,

        [PermissionSeedData("Edit Aircraft", PermissionCategoryEnum.Resource, "Users with this role can edit and ground aircraft.")]
        AircraftEdit = 2,
    }
}