using System;
using System.Collections.Generic;

namespace CoreMultiTenancy.Identity.Entities
{
    /// <summary>
    /// Acts as an entity for a tenant. A tenant has its own user-defined roles along with global defaults.
    /// </summary>
    public class Organization
    {
        public Guid Id { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public byte[] Logo { get; private set; }
        public string LogoFormat { get; private set; }
        public string Title { get; private set; }
        public bool IsActive { get; private set; }
        public bool RequiresConfirmationForNewUsers { get; private set; }
        public DateTime CreationDate { get; private set; }
        public List<UserOrganization> UserOrganizations { get; set; } 
        public List<UserOrganizationRole> UserOrganizationRoles { get; set; }
        public List<Role> Roles { get; set; }
        public Organization() { }

        public Organization(string title, bool requiresConf, Guid ownerId, byte[] logo, string logoFmt)
        {
            Title = title;
            IsActive = true;
            RequiresConfirmationForNewUsers = requiresConf;
            CreationDate = DateTime.UtcNow;
            OwnerUserId = ownerId;
            Logo = logo;
            LogoFormat = logoFmt;
        }
    }
}