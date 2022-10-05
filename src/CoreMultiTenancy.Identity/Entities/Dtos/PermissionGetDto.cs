using AutoMapper;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    [AutoMap(typeof(Permission))]
    public class PermissionGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
    }
}