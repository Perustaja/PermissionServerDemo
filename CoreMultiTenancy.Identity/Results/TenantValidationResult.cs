namespace CoreMultiTenancy.Identity.Results
{
    /// <summary>
    /// Returns information from a tenant validation result indicating whether the tenant is
    /// inactive or whether the user is not authorized to access this tenant. 
    /// </summary>
    public class TenantValidationResult
    {
        public bool Success => (TenantInactive || UserUnauthorized || TenantNotFound) ? false : true;
        public bool TenantNotFound { get; set; }
        public bool TenantInactive{ get; set; }
        public bool UserUnauthorized { get; set; }
        /// <summary>
        /// Returns a TenantValidationResult with no errors.
        /// </summary>
        public static TenantValidationResult SuccessResult => new TenantValidationResult();
    }
}