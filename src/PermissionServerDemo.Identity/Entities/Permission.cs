using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Identity.Entities
{
    /// <summary>
    /// Application-defined permissions that user-defined or default roles have.
    /// </summary>
    public class Permission
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsObsolete { get; set; }
        public string PermCategoryId { get; set; }
        [ForeignKey("PermCategoryId")]
        public PermissionCategory PermCategory { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public Permission() { }
        public Permission(PermissionEnum pEnum) => Id = pEnum.ToString();
    }
}