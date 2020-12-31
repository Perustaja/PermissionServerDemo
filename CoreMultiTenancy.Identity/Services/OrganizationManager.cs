using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using CoreMultiTenancy.Identity.Authorization;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly ILogger<OrganizationManager> _logger;
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IOrganizationInviteService _inviteSvc;

        public OrganizationManager(ILogger<OrganizationManager> logger,
            IUserOrganizationRepository userOrgRepo,
            IOrganizationRepository orgRepo,
            IRoleRepository roleRepo,
            IOrganizationInviteService inviteSvc)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _inviteSvc = inviteSvc ?? throw new ArgumentNullException(nameof(inviteSvc));
        }

        public async Task<string> CreatePermanentInvitationLinkAsync(Guid orgId)
            => await _inviteSvc.CreatePermanentInviteLinkAsync(orgId);

        public async Task<InviteResult> UsePermanentInvitationAsync(User user, string link)
        {
            if (!await _inviteSvc.TryDecodePermanentInviteLinkAsync(link, out var guid))
                return InviteResult.LinkInvalid();

            var org = await _orgRepo.GetByIdAsync(guid);
            if (org != null)
            {
                // link corresponds to a valid Organization, change access and return result
                var res = await GrantAccessAsync(user, org);
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

        public async Task<AccessModifiedResult> GrantAccessAsync(User user, Organization org)
        {
            var record = await _userOrgRepo.GetByIdsAsync(user.Id, org.Id);
            // Check existing record to see its status
            if (record != null)
            {
                if (record.Blacklisted)
                    return new AccessModifiedResult() { UserBlacklisted = true };
                else if (record.AwaitingApproval)
                    return new AccessModifiedResult() { AwaitingConfirmation = true };
                else
                    return new AccessModifiedResult() { ExistingAccess = true };
            }

            // Attempt to save new record
            var accessGrant = new UserOrganization(user.Id, org.Id);
            try
            {
                await _userOrgRepo.AddAsync(accessGrant);
            }
            catch
            {
                _logger.LogError($"Exception while trying to create new UserOrganization. User: {accessGrant.UserId} Organization: {accessGrant.OrganizationId}.");
                var errorResult = new AccessModifiedResult() { ErrorMessage = "An unexpected error has occurred. If the issue persists, contact site administration." };
                return errorResult; // Return an empty result indicating complete failure
            }
            return AccessModifiedResult.SuccessfulResult(org.RequiresConfirmation);
        }

        public async Task<bool> RevokeAccessAsync(Guid userId, Guid orgId)
        {
            var uo = await _userOrgRepo.GetByIdsAsync(userId, orgId);
            if (uo != null)
            {
                try
                {
                    await _userOrgRepo.DeleteAsync(uo);
                }
                catch
                {
                    _logger.LogError($"Exception while trying to delete UserOrganization. UserID: {uo.UserId} OrgId: {uo.OrganizationId}.");
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> UserHasAccessAsync(Guid userId, Guid orgId)
            => await _userOrgRepo.ExistsWithAccessAsync(userId, orgId);
    }
}