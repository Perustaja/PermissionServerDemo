
using PermissionServer.Common;
using PermissionServerDemo.Core.Attributes;

namespace PermissionServerDemo.Core.Authorization
{
    /// <summary>
    /// Main application permissions represented as an enum. Changing underlying string value WILL introduce
    /// breaking changes to database. Each value must have a PermissionSeedData attribute. Use [Obsolete] 
    /// if one is deprecated.
    /// </summary>
    public enum PermissionEnum : byte
    {
        [Permission(PermissionCategoryEnum.Aircraft, "Create Aircraft", "Users with this permission can create new aircraft within the tenant.")]
        AircraftCreate,
        [Permission(PermissionCategoryEnum.Aircraft, "Edit Aircraft", "Users with this permission can edit aircraft within the tenant.")]
        AircraftEdit,
        [Permission(PermissionCategoryEnum.Aircraft, "Delete Aircraft", "Users with this permission can delete aircraft within the tenant.")]
        AircraftDelete,

        [Permission(PermissionCategoryEnum.Roles, "Create Roles", "Users with this permission can create roles within the tenant.")]
        RolesCreate,
        [Permission(PermissionCategoryEnum.Roles, "Edit Roles", "Users with this permission can edit roles within the tenant.")]
        RolesEdit,
        [Permission(PermissionCategoryEnum.Roles, "Delete Roles", "Users with this permission can delete roles within the tenant.")]
        RolesDelete,

        [Permission(PermissionCategoryEnum.Users, "Manage Users' Roles", "Users with this permission can add or remove users' roles within the tenant.")]
        UsersManageRoles,
        [Permission(PermissionCategoryEnum.Users, "Manage Users' Access", "Users with this permission can revoke access for users within the tenant.")]
        UsersManageAccess
    }
}