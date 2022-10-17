using System.ComponentModel.DataAnnotations;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Identity.Entities
{
    /// <summary>
    /// Permission category for grouping and displaying permissions to user.
    /// </summary>
    public class PermissionCategory
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsObsolete { get; set; }
        public List<Permission> Permissions { get; set; }
        public PermissionCategory() { }
        public PermissionCategory(PermissionCategoryEnum pcEnum) => Id = pcEnum.ToString();
    }
}