namespace CoreMultiTenancy.Identity.Results
{
    public class AccessModifiedResult
    {
        public bool Success { get; set; }
        public bool RequiresConfirmation { get; set; }
        public bool AwaitingConfirmation { get; set; }
        public bool ExistingAccess { get; set; }
        public bool UserBlacklisted { get; set; }
        public string ErrorMessage { get; set; }
        public static AccessModifiedResult SuccessfulResult(bool reqConf) 
            => new AccessModifiedResult { Success = true, RequiresConfirmation = reqConf };
    }
}