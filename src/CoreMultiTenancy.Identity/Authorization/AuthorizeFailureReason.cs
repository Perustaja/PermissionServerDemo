namespace CoreMultiTenancy.Identity.Authorization
{
    public enum AuthorizeFailureReason
    {
        Unauthorized, // The tenant exists, but the user does not have access or the required permissions
        TenantNotFound, // No tenant was found based on the tenant id
        PermissionFormat // The permissions passed were unsuccessfully parsed
    }
}