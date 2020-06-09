using System;

namespace CoreMultiTenancy.Identity.Models
{
    public class Location
    {
        public Guid Id { get; private set; }
        public Guid OrganizationId { get; private set; }
        public string Region { get; private set; }
    }
}