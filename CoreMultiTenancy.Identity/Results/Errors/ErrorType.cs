namespace CoreMultiTenancy.Identity.Results.Errors
{
    /// <summary>
    /// An enum with values that represent general application errors.
    /// </summary>
    public enum ErrorType
    {
        Unspecified,
        DomainLogic,
        NotFound,
        KeyExists,
        BadRequest,
    }
}