using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Services
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
            // Retrieve the user and its ClaimsPrincipal from the context
            var user = await _userManager.GetUserAsync(context.Subject) 
                ?? throw new Exception($"{this.GetType().Name}: Unable to find user with ID: {context.Subject.GetSubjectId()}.");
            var principal = await _principalsFactory.CreateAsync(user);

            // Add our custom claims
            var claims = principal.Claims.ToList();
            var tidClaim = new Claim("tid", user.SelectedOrg.ToString());
            claims.Add(tidClaim);
            
            context.AddRequestedClaims(claims);
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subId);

            context.IsActive = user != null;
        }
    }
}