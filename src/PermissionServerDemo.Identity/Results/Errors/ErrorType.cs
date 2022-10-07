namespace PermissionServerDemo.Identity.Results.Errors
{
    /// <summary>
    /// An enum with values that represent general application errors.
    /// </summary>
    public enum ErrorType
    {
        // Use for general purpose errors to return a BadRequest
        Unspecified,
        DomainLogic,
        NotFound,
        KeyExists,
    }
}