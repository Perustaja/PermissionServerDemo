using System;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Models
{
    public class UserOrganization
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public User User { get; set; }
        public Organization Organization { get; set; }
    }
}