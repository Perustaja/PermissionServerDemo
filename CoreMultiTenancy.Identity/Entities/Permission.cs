using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CoreMultiTenancy.Identity.Authorization;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Application-defined permissions that user-defined or default roles have.
    /// </summary>
    public class Permission
    {
        [Key]
        public PermissionEnum Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string PermCategoryId { get; private set; }
        public PermissionCategory PermCategory { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public Permission() { }

        public Permission(PermissionEnum pEnum, string name, string desc, string permCatId)
        {
            Id = pEnum;
            Name = name;
            Description = desc;
            PermCategoryId = permCatId;
        }
    }
}