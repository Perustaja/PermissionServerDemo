using System.ComponentModel.DataAnnotations;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    public class RoleCreateDto
    {
        [MinLength(1)]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionEnum> Permissions { get; set; }
    }
}