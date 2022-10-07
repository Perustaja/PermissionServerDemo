using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace PermissionServerDemo.Identity.Entities.Dtos
{
    [AutoMap(typeof(Role))]
    public class RoleGetDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        [SourceMember(nameof(Role.RolePermissions))]
        public List<PermissionGetDto> Permissions { get; set; }
    }
}