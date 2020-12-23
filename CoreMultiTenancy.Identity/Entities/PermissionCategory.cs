using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Permission category for grouping and displaying permissions to user. One to one with Permission.
    /// </summary>
    public class PermissionCategory
    {
        [Key]
        public string Id { get; private set; }
        [StringLength(50)]
        public string Description { get; private set; }
        public PermissionCategory() { }

        public PermissionCategory(string id, string desc)
        {
            Id = id;
            Description = desc;
        }
    }
}