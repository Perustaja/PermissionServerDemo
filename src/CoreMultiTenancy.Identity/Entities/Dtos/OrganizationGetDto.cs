using AutoMapper;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    [AutoMap(typeof(Organization))]
    public class OrganizationGetDto
    {
        public Guid Id { get; private set; }
        public string LogoUri { get; private set; }
        public string Title { get; private set; }
        public bool IsActive { get; private set; }
    }
}