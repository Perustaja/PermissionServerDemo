using AutoMapper;
using PermissionServer.Common;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Identity.Entities.Dtos
{
    [AutoMap(typeof(Permission<PermissionEnum, PermissionCategoryEnum>))]
    public class PermissionGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
    }
}