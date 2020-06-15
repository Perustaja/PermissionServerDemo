using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Models
{
    public class User : IdentityUser<Guid>
    {
        // Join tables
        public List<UserOrganization> UserOrganizations { get; set; }        
    }
}