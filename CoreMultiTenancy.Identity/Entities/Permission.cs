using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Application-defined permissions that user-defined or default roles have.
    /// </summary>
    public class Permission
    {
        [Key]
        public string Id { get; private set; }
        public string PermCategoryId { get; private set; }
        public PermissionCategory PermCategory { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public Permission() { }

        public Permission(string id, string permCatId)
        {
            Id = id;
            PermCategoryId = permCatId;
        }
    }
}