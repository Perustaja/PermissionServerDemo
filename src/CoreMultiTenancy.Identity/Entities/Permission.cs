using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Application-defined permissions that user-defined or default roles have.
    /// </summary>
    public class Permission
    {
        [Key]
        public PermissionEnum Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsObsolete { get; set; }
        public bool VisibleToUser { get; set; }
        public PermissionCategoryEnum PermCategoryId { get; set; }
        [ForeignKey("PermCategoryId")]
        public PermissionCategory PermCategory { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public Permission() { }

        public Permission(PermissionEnum pEnum) => Id = pEnum;
    }
}