using System;
using System.Collections.Generic;

namespace PermissionServerDemo.Identity.Entities
{
    /// <summary>
    /// Acts as an entity for a tenant. A tenant has its own user-defined roles along with global defaults.
    /// </summary>
    public class Organization
    {
        public Guid Id { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public string LogoUri { get; private set; }
        public string Title { get; private set; }
        public bool IsActive { get; private set; }
        public bool RequiresConfirmationForNewUsers { get; private set; }
        public DateTime CreationDate { get; private set; }
        public List<UserOrganization> UserOrganizations { get; set; }
        public List<UserOrganizationRole> UserOrganizationRoles { get; set; }
        public Organization() { }
        public Organization(string title, bool requiresConf, Guid ownerId, string logoUri)
        {
            Id = new Guid();
            Title = title;
            IsActive = true;
            RequiresConfirmationForNewUsers = requiresConf;
            CreationDate = DateTime.UtcNow;
            OwnerUserId = ownerId;
            LogoUri = logoUri;
        }

        /// <summary>
        /// For demo purposes only
        /// </summary>
        public Organization(Guid id, string title, bool requiresConf, Guid ownerId, string logoUri)
        {
            Id = id;
            Title = title;
            IsActive = true;
            RequiresConfirmationForNewUsers = requiresConf;
            CreationDate = DateTime.UtcNow;
            OwnerUserId = ownerId;
            LogoUri = logoUri;
        }
    }
}