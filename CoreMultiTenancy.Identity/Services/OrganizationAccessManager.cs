using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
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
            if (!await _userOrgRepo.ExistsAsync(user.Id, org.Id))
                return new AccessModifiedResult() { ExistingAccess = true };
            // TODO: check if user is on blacklist

            // Attempt to save new record
            var accessGrant = new UserOrganization(user.Id, org.Id);
            try
            {
                await _userOrgRepo.AddAsync(accessGrant);
            }
            catch 
            {
                _logger.LogError($"Exception while trying to create new UserOrganization. User: {accessGrant.UserId} Organization: {accessGrant.OrganizationId}.");
                return new AccessModifiedResult(); // Return an empty result indicating complete failure
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
            => await _userOrgRepo.ExistsAsync(userId, orgId);
    }
}