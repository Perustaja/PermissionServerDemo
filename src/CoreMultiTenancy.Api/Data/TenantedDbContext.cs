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
        private readonly Guid _demoMyTenantId;
        private readonly Guid _demoOtherTenantId;

        public TenantedDbContext(DbContextOptions<TenantedDbContext> options,
            IConfiguration config,
            ITenantProvider tenantProvider)
            : base(options)
        {
            _tenant = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
            _demoMyTenantId = Guid.Parse(config["DemoMyTenantId"]);
            _demoOtherTenantId = Guid.Parse(config["DemoOtherTenantId"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // tenancy filter
            modelBuilder.Entity<Aircraft>().HasQueryFilter(ac => EF.Property<string>(ac, "tenantId") == tenantId);

            SeedDatabaseForDemo(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SeedDatabaseForDemo(ModelBuilder modelBuilder)
        {
            var tenant1Ac = new Aircraft("N772GK", _demoMyTenantId, "N772GK.jpg");
            var tenant2Ac = new Aircraft("N5342K", _demoOtherTenantId, "N5342K.jpg");

            modelBuilder.Entity<Aircraft>().HasData(tenant1Ac, tenant2Ac);
        }
    }
}