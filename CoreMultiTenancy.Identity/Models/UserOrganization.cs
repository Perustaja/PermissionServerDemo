using System;

namespace CoreMultiTenancy.Identity.Models
{
    public class UserOrganization
    {
        public Guid UserId { get; private set; }
        public Guid OrganizationId { get; private set; }
        public bool AwaitingApproval { get; private set; } = true;
        public bool Blacklisted { get; private set; }
        public string InternalNotes { get; private set; }
        public DateTime DateSubmitted { get; private set; }
        public DateTime? DateApproved { get; private set; }
        public DateTime? DateBlacklisted { get; private set; }
        public User User { get; set; }
        public Organization Organization { get; set; }
        public UserOrganization() { }
        public UserOrganization(Guid userId, Guid orgId)
        {
            UserId = userId;
            OrganizationId = orgId;
            DateSubmitted = DateTime.Today;
        }
        public void Approve() 
        {
            AwaitingApproval = false;
            DateApproved = DateTime.Today;
        }
        public void Blacklist()
        {
            Blacklisted = true;
            AwaitingApproval = true;
            DateBlacklisted = DateTime.Today;
        }
        public void SetInternalNotes(string notes) => InternalNotes = notes;
    }
}