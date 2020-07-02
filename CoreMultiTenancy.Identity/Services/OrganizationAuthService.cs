using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationAuthService : ITenantAuthService<Guid>
    {
        private readonly ILogger<OrganizationAuthService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IAccessRevokedEventRepository _eventRepo;
        private readonly IOrganizationRepository _orgRepo;
        public OrganizationAuthService(ILogger<OrganizationAuthService> logger,
        UserManager<User> userManager, IAccessRevokedEventRepository eventRepo,
        IOrganizationRepository orgRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _eventRepo = eventRepo ?? throw new ArgumentNullException(nameof(eventRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
        }
        public async Task<bool> ValidateUserAsync(Guid subId)
        {
            bool isValid = false;
            // Initially check if record even exists for user, using subject id to avoid UserManager overhead
            var accessEvent = await _eventRepo.GetByUserIdAsync(subId);
            if (accessEvent == null)
                isValid = true;

            // If it does, get the user associated with the id
            var user = await _userManager.FindByIdAsync(subId.ToString()) 
                ?? throw new Exception($"{this.GetType().Name}: Unable to find user with ID: {subId}.");

            // If the User is currently "logged in" to the same Organization, it is now unauthorized
            if (accessEvent.OrganizationId != user.SelectedOrg)
                isValid = true; // Portal will prevent invalid selection in the future
            return isValid;
        }
        public async Task UserAccessRevokedAsync(Guid userId, Guid orgId)
        {
            // Get user associated with id
            var user = await _userManager.FindByIdAsync(userId.ToString()) 
                ?? throw new Exception($"{this.GetType().Name}: Unable to find user with ID: {userId}.");
            // if its selectedorg is == orgId, make record and add
            if (user.SelectedOrg == orgId)
                await _eventRepo.AddAsync(userId, orgId);
        }
    }
}