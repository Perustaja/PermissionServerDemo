using CoreMultiTenancy.Identity.Attributes;

namespace CoreMultiTenancy.Identity.Authorization
{
    /// <summary>
    /// Main application permissions represented as an enum. Changing underlying byte value WILL introduce
    /// breaking changes to database.
    /// </summary>
    public enum PermissionEnum : byte
    {
        Default = 0,

        [SeedData("Create Aircraft")]
        AircraftCreate = 1,

        [SeedData("Edit Aircraft", "Users with this role can edit and ground aircraft.")]
        AircraftEdit = 2,
    }
}