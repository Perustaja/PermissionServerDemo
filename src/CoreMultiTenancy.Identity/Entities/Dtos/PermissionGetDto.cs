using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    [AutoMap(typeof(Permission))]
    public class PermissionGetDto
    {
        public PermissionEnum Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        [SourceMember(nameof(Permission.PermCategory))]
        public PermissionCategoryGetDto PermissionCategory { get; set; }
    }
}