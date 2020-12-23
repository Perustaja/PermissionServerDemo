using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationAccessManager : IOrganizationAccessManager
    {
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly ILogger<OrganizationAccessManager> _logger;
        public OrganizationAccessManager(ILogger<OrganizationAccessManager> logger,
            IUserOrganizationRepository userOrgRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
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