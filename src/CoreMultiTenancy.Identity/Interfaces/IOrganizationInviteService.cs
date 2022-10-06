namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IOrganizationInviteService
    {
        /// <summary>
        /// Returns a permanent invitation link based on the id passed.
        /// <summary>
        Task<string> CreatePermanentInviteLinkAsync(Guid orgId);
        /// <summary>
        /// Attempts to decode a permanent invitation link, returning true if the invitation code was valid.
        /// </summary>
        /// <param name="code">The code created by this service's CreatePermanentInviteLink method.</param>
        /// <param name="guid">A guid variable that will be set with the accompanying organization id if successful.</param>
        /// <returns></returns>
        Task<bool> TryDecodePermanentInviteLinkAsync(string code, out Guid guid);
    }
}