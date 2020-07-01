using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PortalInteractionResponseGenerator : AuthorizeInteractionResponseGenerator
    {
        private readonly ILogger<AuthorizeInteractionResponseGenerator> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ITenantInfoValidator<Guid> _validator;
        public PortalInteractionResponseGenerator(ISystemClock clock, 
        ILogger<AuthorizeInteractionResponseGenerator> logger,
        IConsentService consentService, IProfileService profileService,
        UserManager<User> userManager,
        ITenantInfoValidator<Guid> validator)
            : base(clock, logger, consentService, profileService) 
            {
                _logger = logger;
                _userManager = userManager;
                _validator = validator;
            }

        public override async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest req,
        ConsentResponse consent = null)
        {
            // Send to base first, ensure that the user is logged in and otherwise valid
            var response = await base.ProcessInteractionAsync(req, consent);
            var readyToAdvance = !(response.IsConsent || response.IsError || response.IsLogin);

            // Get the user associated with this request and validate it
            var user = await _userManager.GetUserAsync(req.Subject) 
                ?? throw new Exception($"{this.GetType().Name}: Unable to find user with ID: {req.Subject.GetSubjectId()}.");
            var vResult = _validator.ValidateSelectedOrganization(user.Id, user.SelectedOrg);

            // Respond based on possible validation errors
            if (vResult.TenantNotFound)
            {
                _logger.LogError($"User: {user.Id} SelectedOrg: {user.SelectedOrg} Organization doesn't exist in database. Redirecting to portal.");
                return new InteractionResponse() 
                { 
                    RedirectUrl = "/Portal",
                    ErrorDescription = "There was an error accessing the selected company. If the problem persists, contact site administration.",
                };
            }
            if (vResult.TenantInactive)
            {
                _logger.LogInformation($"User: {user.Id} SelectedOrg: {user.SelectedOrg} Organization inactive. Redirecting to portal.");
                return new InteractionResponse() 
                { 
                    RedirectUrl = "/Portal",
                    ErrorDescription = "The selected company is inactive. Please contact the company's administation for further information.",
                };
            }
            if (vResult.UserUnauthorized)
            {
                _logger.LogInformation($"User: {user.Id} SelectedOrg: {user.SelectedOrg} User unauthorized to access this Organization. Redirecting to portal.");
                return new InteractionResponse() 
                { 
                    RedirectUrl = "/Portal",
                    ErrorDescription = "You no longer have access to this company. Please contact the company's administation for further information.",
                };
            }

            return response; // Continue on normally
        }
    }
}