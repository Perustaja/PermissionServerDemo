using System;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Services
{
    public class CachedOrganizationInfoValidator : ITenantInfoValidator<Guid>
    {
        private readonly ITenantInfoCache<Organization, Guid> _tenantInfoCache;
        public CachedOrganizationInfoValidator(ITenantInfoCache<Organization, Guid> tenantInfoCache)
        {
            _tenantInfoCache = tenantInfoCache;
        }
        public TenantValidationResult ValidateSelectedOrganization(Guid userId, Guid selectedOrg)
        {
            var org = _tenantInfoCache.GetOrganization(selectedOrg);
            // Does the organization exist?
            if (org == null)
                return new TenantValidationResult() { TenantNotFound = true };
            // Is the organization active?
            if (!org.IsActive)
                return new TenantValidationResult() { TenantInactive = true };
            // Is the user authorized to access this organization's data?
            if (_tenantInfoCache.GetUserOrganization(userId, selectedOrg) == null)
                return new TenantValidationResult() { UserUnauthorized = true };
            
            return TenantValidationResult.SuccessResult;
        }
    }
}