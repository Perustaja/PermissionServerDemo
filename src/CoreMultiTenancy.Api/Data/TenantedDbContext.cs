using System;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Interfaces;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Data
{
    public class TenantedDbContext : DbContext
    {
        DbSet<Aircraft> Aircraft { get; set; }
        public string tenantId => _tenant.Id;
        private readonly Tenant _tenant;

        public TenantedDbContext(DbContextOptions<TenantedDbContext> options,
            IConfiguration config,
            ITenantProvider tenantProvider)
            : base(options)
        {
            _tenant = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // tenancy filter
            mb.Entity<Aircraft>().HasQueryFilter(ac => EF.Property<string>(ac, "tenantId") == tenantId);
        }
    }
}