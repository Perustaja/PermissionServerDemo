using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace PermissionServerDemo.Identity.Entities.Dtos
{
    [AutoMap(typeof(PermissionCategory))]
    public class PermissionCategoryGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [SourceMember(nameof(PermissionCategory.Permissions))]
        public List<PermissionGetDto> Permissions { get; set; }
    }
}