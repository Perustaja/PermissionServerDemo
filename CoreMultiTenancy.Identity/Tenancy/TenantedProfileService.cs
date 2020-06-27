using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Tenancy
{
    public class TenantedProfileService : IProfileService
    {
        private readonly ILogger<TenantedProfileService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _principalsFactory;
        public TenantedProfileService(ILogger<TenantedProfileService> logger,
        UserManager<User> userManager,
        IUserClaimsPrincipalFactory<User> principalsFactory)
        {
            _logger = logger;
            _userManager = userManager;
            _principalsFactory = principalsFactory;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(_logger);
            // Use the information we have to get the user associated with the subject and then its claims
            var subId = context.Subject.GetSubjectId();
            if (!Guid.TryParse(subId, out var parsedId))
                throw new Exception($"TenantedProfileService could not parse SubjectId: {subId} into usable Guid.");
            
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == parsedId) ?? throw new Exception($"Unable to find user with ID: {parsedId}.");
            var principal = await _principalsFactory.CreateAsync(user);

            // Append our tenancy id claim
            var tidClaim = context.Subject.Claims.FirstOrDefault(c => c.Type == "tid");
            if (tidClaim != null)
                principal.Claims.Append(tidClaim);
            
            context.AddRequestedClaims(principal.Claims);
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subId);

            context.IsActive = user != null;
        }
    }
}