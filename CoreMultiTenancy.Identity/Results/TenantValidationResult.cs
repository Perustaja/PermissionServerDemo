namespace CoreMultiTenancy.Identity.Results
{
    /// <summary>
    /// Returns information indicating whether User has sufficient privileges to access
    /// its SelectedOrg.
    /// </summary>
    public class TenantValidationResult
    {
        public bool Success { get; set; }
        public bool UserUnauthorized { get; set; }
        /// <summary>
        /// Returns a TenantValidationResult with no errors.
        /// </summary>
        public static TenantValidationResult SuccessResult => new TenantValidationResult() { Success = true };
        public static TenantValidationResult UnauthorizedResult => new TenantValidationResult() { UserUnauthorized = true };
    }
}