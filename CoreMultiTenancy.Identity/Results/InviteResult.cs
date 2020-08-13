namespace CoreMultiTenancy.Identity.Results
{
    /// <summary>
    /// Contains information on the outcome when an invitation link is used.
    /// <summary>
    public class InviteResult
    {
        public bool Success { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public static InviteResult ImmediateSuccess(string orgName)
            => Succeeded($"You have successfully been granted access to {orgName}. Changes should be reflected in your portal immediately.");
        public static InviteResult RequiresConfirmation(string orgName)
            => Succeeded($"The Organization {orgName} requires manual confirmation of all users, a request has been sent and you will receive an email based on their decision.");
        public static InviteResult LinkExpired()
            => Failed("This link has expired. Please contact your company for a valid invitation link.");
        public static InviteResult LinkInvalid()
            => Failed("The link provided was not valid. Please contact your company for a valid invitation link.");
        public static InviteResult ExistingAccess(string orgName)
            => Failed($"You already have existing access to the company {orgName}.");
        public static InviteResult AwaitingConfirmation(string orgName)
            => Failed($"Your previous request to join {orgName} still requires confirmation. An email will be sent to your email based upon their decision. Please contact the company for further information.");
        public static InviteResult Blacklisted()
            => Failed("You have been marked by this company as unable to request access. Contact the company for further information.");
        private static InviteResult Succeeded(string msg)
        {
            return new InviteResult()
            {
                Success = true,
                SuccessMessage = msg,
            };
        }
        private static InviteResult Failed(string msg)
        {
            return new InviteResult()
            {
                Success = false,
                ErrorMessage = msg,
            };
        }
    }
}