using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationInviteService : IOrganizationInviteService
    {
        private readonly IOrganizationRepository _orgRepo;
        private readonly IOrganizationAccessManager _orgAccessManager;
        public OrganizationInviteService(IOrganizationRepository orgRepo,
            IOrganizationAccessManager orgAccessManager)
        {
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _orgAccessManager = orgAccessManager ?? throw new ArgumentNullException(nameof(orgAccessManager));
        }
        public string CreatePermInvitationLink(Guid orgId) => EncodePerm(orgId);
        public async Task<InviteResult> UsePermInvitationLink(User user, string link)
        {
            if (!TryDecodePerm(link, out var guid))
                return InviteResult.LinkInvalid();
            
            var org = await _orgRepo.GetByIdAsync(guid);
            if (org != null)
            {
                // link corresponds to a valid Organization, change access and return result
                var res = await _orgAccessManager.GrantAccessAsync(user, org);
                if (res.Success)
                    return res.RequiresConfirmation ? InviteResult.RequiresConfirmation(org.Title) : InviteResult.ImmediateSuccess(org.Title);
                else if (res.UserBlacklisted)
                    return InviteResult.Blacklisted();
                else if (res.ExistingAccess)
                    return InviteResult.ExistingAccess(org.Title);
            }
            // No Organization found or some miscellaneous failure occurred
            return InviteResult.LinkInvalid();
        }
        private string EncodePerm(Guid id) =>
            Convert.ToBase64String(id.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);

        private bool TryDecodePerm(string code, out Guid guid)
        {
            var originalCode = code.Replace("_", "/").Replace("-", "+") + "==";
            // Try and convert, if not return false
            try
            {
                byte[] buffer = Convert.FromBase64String(originalCode);
                guid = new Guid(buffer);
                return true;
            }
            catch
            {
                guid = Guid.Empty;
                return false;
            }
        }
    }
}