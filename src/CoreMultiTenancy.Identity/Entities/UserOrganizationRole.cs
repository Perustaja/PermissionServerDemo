using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity
{
    /// <summary>
    /// Three-way join table that represents the roles a user has, specific to each tenant.
    /// </summary>
    public class UserOrganizationRole
    {
        public Guid UserId { get; private set; }
        public Guid OrgId { get; private set; }
        public Guid RoleId { get; private set; }
        public User User { get; set; }
        public Organization Organization { get; set; }
        public Role Role { get; set; }
        public UserOrganizationRole() { }
        public UserOrganizationRole(Guid userId, Guid orgId, Guid roleId)
        {
            UserId = userId;
            OrgId = orgId;
            RoleId = roleId;
        }
    }
}