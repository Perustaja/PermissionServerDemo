using System;
using System.Diagnostics;
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
    /// Ensures that 
    /// </summary>
    public class PortalInteractionResponseGenerator : AuthorizeInteractionResponseGenerator
    {
        private readonly ILogger<AuthorizeInteractionResponseGenerator> _logger;
        public PortalInteractionResponseGenerator(ISystemClock clock, 
        ILogger<AuthorizeInteractionResponseGenerator> logger,
        IConsentService consentService, IProfileService profileService)
            : base(clock, logger, consentService, profileService) 
            {
                _logger = logger;
            }

        public override async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest req,
        ConsentResponse consent = null)
        {
            var sw = new Stopwatch();
            sw.Start();
            // Send to base first, ensure that the user is logged in and otherwise valid
            var response = await base.ProcessInteractionAsync(req, consent);
            var readyToAdvance = !(response.IsConsent || response.IsError || response.IsLogin);

            if (req.Subject.HasClaim(c => c.Type == "tid" && c.Value == Guid.Empty.ToString()))
            // Get the user associated with this request and validate it
            
            sw.Stop();
            _logger.LogDebug($"{this.GetType().Name}: check completed in {sw.ElapsedMilliseconds}ms.");
            return response; // Continue on normally
        }
    }
}