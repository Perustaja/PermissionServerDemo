using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    public class RoleCreateDto
    {
        [MinLength(1)]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Permissions { get; set; }
    }
}