using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Api.Entities
{
    public class Aircraft
    {
        [Key]
        public string RegNumber { get; private set; }
        public bool IsGrounded { get; private set; }
        public Aircraft() { }
        public Aircraft(string regNum) => RegNumber = regNum;
        public void Ground() => IsGrounded = true;
    }
}