using System;
using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Api.Entities
{
    public class Aircraft
    {
        [Key]
        public string RegNumber { get; private set; }
        public string ThumbnailUri { get; private set; }
        public Guid TenantId { get; private set;  }
        public bool IsGrounded { get; private set; }
        public Aircraft() { }
        public Aircraft(string regNum, Guid tenantId, string thumbUri) 
        {
            RegNumber = regNum;
            TenantId = tenantId;
            ThumbnailUri = thumbUri;
        }
        public void Ground() => IsGrounded = true;
    }
}