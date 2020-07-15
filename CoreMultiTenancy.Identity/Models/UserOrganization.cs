using System;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Models
{
    public class UserOrganization
    {
        public Guid UserId { get; private set; }
        public Guid OrganizationId { get; private set; }
        public User User { get; set; }
        public Organization Organization { get; set; }
        public UserOrganization() { }
        public UserOrganization(Guid userId, Guid orgId)
        {
            UserId = userId;
            OrganizationId = orgId;
        }
    }
}