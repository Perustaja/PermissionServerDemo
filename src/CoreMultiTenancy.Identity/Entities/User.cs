using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Entities
{
    public class User : IdentityUser<Guid>
    {
        [PersonalData]
        public string FirstName { get; private set; }
        [PersonalData]
        public string LastName { get; private set; }
        public List<UserOrganization> UserOrganizations { get; set; }
        public List<UserOrganizationRole> UserOrganizationRoles { get; set; }
        public User() { }

        public User(string fName, string lName, string email)
        {
            Id = Guid.NewGuid();
            FirstName = fName;
            LastName = lName;
            UserName = email;
            Email = email;
        }

        /// <summary>
        /// For demo purposes only
        /// </summary>
        public User(Guid id, string fName, string lName, string email)
        {
            Id = id;
            FirstName = fName;
            LastName = lName;
            UserName = email;
            Email = email;
        }

        /// <summary>
        /// Updates the User's associated name information. Silently fails if strings are empty.
        /// </summary>
        public void UpdateName(string fName, string lName)
        {
            if (!String.IsNullOrWhiteSpace(fName) && !String.IsNullOrWhiteSpace(lName))
            {
                FirstName = fName;
                LastName = lName;
            }
        }
    }
}