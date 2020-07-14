using System;

namespace CoreMultiTenancy.Identity.Models
{
    public class OrganizationAccessToken
    {
        public string Value { get; set; }
        public Guid OrganizationId { get; private set; }
        public string IntendedEmailAddress { get; private set; }
        public DateTime Expiration { get; private set; }
        public OrganizationAccessToken() { }
        public OrganizationAccessToken(string value, Guid orgId, string email, DateTime expiration)
        {
            Value = value;
            OrganizationId = orgId;
            IntendedEmailAddress = email;
            Expiration = expiration;
        }
    }
}