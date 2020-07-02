using System;
using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Identity.Models
{
    /// <summary>
    /// Created when an Organization revokes a User's access while its SelectedOrg property is set
    /// to the Id of the Organization. This immediately prevents removed Users still scoped to their data
    /// from accessing it. Only one event should exist for any one User.
    /// </summary>
    public class AccessRevokedEvent
    {
        [Key]
        public Guid UserId { get; private set; }
        public Guid OrganizationId { get; private set; }
        public AccessRevokedEvent() { } // Required by EF
        public AccessRevokedEvent(Guid userId, Guid orgId)
        {
            UserId = userId;
            OrganizationId = orgId;
        }
    }
}