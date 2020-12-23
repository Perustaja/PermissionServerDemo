namespace CoreMultiTenancy.Api.Entities
{
    public class Aircraft
    {
        public string RegNumber { get; private set; }
        public bool IsGrounded { get; private set; }
        public Aircraft() { } // Required by EF Core
        public Aircraft(string regNum) => RegNumber = regNum;
        public void Ground() => IsGrounded = true;
    }
}