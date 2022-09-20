using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    [AutoMap(typeof(Permission))]
    public class PermissionGetDto
    {
        public PermissionEnum Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool VisibleToUser { get; set; }
        public PermissionCategoryEnum PermCategoryId { get; set; }
    }
}