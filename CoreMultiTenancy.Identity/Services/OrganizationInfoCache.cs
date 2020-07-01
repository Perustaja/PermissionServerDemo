using System;
using CoreMultiTenancy.Identity.Data;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationInfoCache : ITenantInfoCache<Organization, Guid>
    {
        // Load up DbSets we want
        // Set up time interval for refresh
        private readonly ApplicationDbContext _context;
        public OrganizationInfoCache(ApplicationDbContext context)
        {
            _context = context;
        }
        public Organization GetOrganization(Guid id)
        {
            throw new NotImplementedException();
        }
        public UserOrganization GetUserOrganization(Guid userId, Guid tenantId)
        {
            throw new NotImplementedException();
        }
    }
}