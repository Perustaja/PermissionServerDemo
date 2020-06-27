using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public Guid SelectedOrg { get; private set; }
        public List<UserOrganization> UserOrganizations { get; set; }
        public User() {} // Required by EF Core
        public User(string fName, string lName, string email)
        {
            Id = Guid.NewGuid();
            FirstName = fName;
            LastName = lName;
            UserName = email;
            Email = email;
            SelectedOrg = Guid.Empty;
        }
    }
}