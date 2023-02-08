using AutoMapper;
using AutoMapper.Configuration.Annotations;
using PermissionServer.Common;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Identity.Entities.Dtos
{
    [AutoMap(typeof(PermissionCategory<PermissionEnum, PermissionCategoryEnum>))]
    public class PermissionCategoryGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [SourceMember(nameof(PermissionCategory<PermissionEnum, PermissionCategoryEnum>.Permissions))]
        public List<PermissionGetDto> Permissions { get; set; }
    }
}