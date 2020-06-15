using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Identity.Tenancy
{
    public class TenantedProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _principalsFactory;
        public TenantedProfileService(UserManager<User> userManager,
        IUserClaimsPrincipalFactory<User> principalsFactory)
        {
            _userManager = userManager;
            _principalsFactory = principalsFactory;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            // Use the information we have to get the user associated with the subject and then its claims
            var subId = context.Subject.GetSubjectId();
            if (!Guid.TryParse(subId, out var parsedID))
                throw new Exception($"TenantedProfileService could not parse SubjectId: {subId} into usable Guid.");
            
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == parsedID);
            var principal = await _principalsFactory.CreateAsync(user);
            var claims = principal.Claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();

            // Finally, inject our custom claims if desired and set IssuedClaims.
            var tidClaim = context.Subject.Claims.FirstOrDefault(c => c.Type == "tid");
            if (tidClaim != null)
                claims.Add(tidClaim);
            
            context.IssuedClaims = claims;
        }
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subId);

            context.IsActive = user != null;
        }
    }
}