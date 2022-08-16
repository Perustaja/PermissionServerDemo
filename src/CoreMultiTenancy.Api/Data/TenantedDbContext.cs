using System;
using System.Threading;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Interfaces;
using CoreMultiTenancy.Api.Tenancy;
using CoreMultiTenancy.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Data
{
    public class TenantedDbContext : DbContext, IUnitOfWork
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

        // Note that SaveChangesAsync() works with most providers to rollback by default 
        // on failure. In other words, it's only being exposed so that multiple operations
        // may be performed in a commit by services. No complicated methods are required for this basic
        // transactional behavior. EF may have proposed best practices so just follow those. 
        public async Task<int> Commit(CancellationToken cancellationToken = default)
            => await SaveChangesAsync();
            
        private void SeedDatabaseForDemo(ModelBuilder modelBuilder)
        {
            var tenant1Ac = new Aircraft("N772GK", _demoMyTenantId, "N772GK.jpg");
            var tenant2Ac = new Aircraft("N5342K", _demoOtherTenantId, "N5342K.jpg");

            modelBuilder.Entity<Aircraft>().HasData(tenant1Ac, tenant2Ac);
        }
    }
}