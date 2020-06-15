// using System.Threading.Tasks;
// using IdentityServer4.Models;
// using IdentityServer4.ResponseHandling;
// using IdentityServer4.Services;
// using IdentityServer4.Validation;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.Extensions.Logging;

// namespace CoreMultiTenancy.Identity.Tenancy
// {
//     public class PortalInteractionResponseGenerator : AuthorizeInteractionResponseGenerator
//     {
//         public PortalInteractionResponseGenerator(ISystemClock clock, 
//         ILogger<AuthorizeInteractionResponseGenerator> logger,
//         IConsentService consentService, IProfileService profileService)
//             : base(clock, logger, consentService, profileService) { }

//         public override async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest req,
//         ConsentResponse consent = null)
//         {
//             var response = await base.ProcessInteractionAsync(req, consent);
//             // Has the user completed the login process?
//             var readyToAdvance = !(response.IsConsent || response.IsError || response.IsLogin);
            
//             // If subject has a default TenantId Claim value, redirect to portal
//             if (readyToAdvance && req.Subject.HasClaim(c => c.Type == "tid" && c.Value == "0"))
//                 return new InteractionResponse() { RedirectUrl = "/Portal" };
//             return response; // Continue on as normal if not
//         }
//     }
// }