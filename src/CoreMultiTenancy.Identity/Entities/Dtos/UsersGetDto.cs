namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    public class UsersGetDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserOrganizationGetDto UserOrganization { get; set; }
        public List<RoleGetDto> Roles { get; set; }
    }
}