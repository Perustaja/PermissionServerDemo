using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
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
            var guid = DecodePerm(link);
            var org = await _orgRepo.GetByIdAsync(guid);
            if (org == null)
                return InviteResult.LinkInvalid();

            // link corresponds to a valid Organization, change access and return result
            var res = await _orgAccessManager.GrantAccessAsync(user, org);
            if (res.Success)
                return res.RequiresConfirmation ? InviteResult.RequiresConfirmation(org.Title) : InviteResult.ImmediateSuccess(org.Title);
            else if (res.UserBlacklisted)
                return InviteResult.Blacklisted();
            else if (res.ExistingAccess)
                return InviteResult.ExistingAccess(org.Title);
            else
                return InviteResult.LinkInvalid();
        }
        private string EncodePerm(Guid id) =>
            Convert.ToBase64String(id.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);

        private Guid DecodePerm(string code)
        {
            var originalCode = code.Replace("_", "/").Replace("-", "+") + "==";
            byte[] buffer = Convert.FromBase64String(originalCode);
            return new Guid(buffer);
        }
    }
}