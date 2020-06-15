using System;
using System.Collections.Generic;

namespace CoreMultiTenancy.Identity.Models
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        // Join tables
        public List<UserOrganization> UserOrganizations { get; set; }        
    }
}