using System.ComponentModel.DataAnnotations;
using CoreMultiTenancy.Identity.Authorization;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Permission category for grouping and displaying permissions to user. One to one with Permission.
    /// </summary>
    public class PermissionCategory
    {
        [Key]
        public PermissionCategoryEnum Id { get; set; }
        public string Name { get; set; }
        public bool IsObsolete { get; set; }
        public PermissionCategory() { }
        public PermissionCategory(PermissionCategoryEnum pcEnum) => Id = pcEnum;
    }
}