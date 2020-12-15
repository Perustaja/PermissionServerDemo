namespace CoreMultiTenancy.Identity.Results
{
    /// <summary>
    /// Contains information about whether the email was approved and pushed to queue, NOT whether it was successfully was sent or not.
    /// </summary>
    public class AccountEmailResult
    {
        public AccountEmailResult(bool approved, string msg) 
        {
            Approved = approved;
            Message = msg;
        }
        public bool Approved { get; set; }
        public string Message { get; set; }
    }
}