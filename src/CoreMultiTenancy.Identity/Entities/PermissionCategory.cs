using System.ComponentModel.DataAnnotations;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Permission category for grouping and displaying permissions to user.
    /// </summary>
    public class PermissionCategory
    {
        [Key]
        public PermissionCategoryEnum Id { get; set; }
        public string Name { get; set; }
        public bool VisibleToUser { get; set; }
        public bool IsObsolete { get; set; }
        public List<Permission> Permissions { get; set; }
        public PermissionCategory() { }
        public PermissionCategory(PermissionCategoryEnum pcEnum) => Id = pcEnum;
    }
}