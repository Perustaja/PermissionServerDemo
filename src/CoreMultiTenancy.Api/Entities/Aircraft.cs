using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Api.Entities
{
    public class Aircraft
    {
        [Key]
        public string RegNumber { get; private set; }
        public string TenantId { get; private set;  }
        public bool IsGrounded { get; private set; }
        public Aircraft() { }
        public Aircraft(string regNum, string tenantId) 
        {
            RegNumber = regNum;
            TenantId = tenantId;
        }
        public void Ground() => IsGrounded = true;
    }
}