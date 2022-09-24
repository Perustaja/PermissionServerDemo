using AutoMapper;
using AutoMapper.Configuration.Annotations;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    [AutoMap(typeof(PermissionCategory))]
    public class PermissionCategoryGetDto
    {
        public PermissionCategoryEnum Id { get; set; }
        public string Name { get; set; }
        [SourceMember(nameof(PermissionCategory.Permissions))]
        public List<PermissionGetDto> Permissions { get; set; }
    }
}