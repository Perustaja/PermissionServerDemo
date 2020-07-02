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
        private readonly ITenantAuthService<Guid> _validator;
        public PortalInteractionResponseGenerator(ISystemClock clock, 
        ILogger<AuthorizeInteractionResponseGenerator> logger,
        IConsentService consentService, IProfileService profileService,
        UserManager<User> userManager,
        ITenantAuthService<Guid> validator)
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

            return response; // Continue on normally
        }
    }
}