using PermissionServerDemo.Identity.Interfaces;

namespace PermissionServerDemo.Identity.Services
{
    /// <summary>
    /// This is a substitution based invite service which serves as a naive implementation. It should
    /// not be used in production as it poses a security risk.
    /// </summary>
    public class OrganizationInviteService : IOrganizationInviteService
    {
        public Task<string> CreatePermanentInviteLinkAsync(Guid orgId) => 
            Task.FromResult(EncodePermanent(orgId));

        public Task<bool> TryDecodePermanentInviteLinkAsync(string code, out Guid guid)
        {
            var originalCode = code.Replace("_", "/").Replace("-", "+") + "==";
            // Try and convert, if not return false
            try
            {
                byte[] buffer = Convert.FromBase64String(originalCode);
                guid = new Guid(buffer);
                return Task.FromResult(true);
            }
            catch
            {
                guid = Guid.Empty;
                return Task.FromResult(false);
            }
        }

        private string EncodePermanent(Guid id) =>
            Convert.ToBase64String(id.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);
    }
}