using PermissionServerDemo.Api.Entities;
using PermissionServerDemo.Core.Interfaces;
using PermissionServerDemo.Core.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace PermissionServerDemo.Api.Data
{
    public class TenantedDbContext : DbContext, IUnitOfWork
    {
        private readonly IConfiguration _config;
        DbSet<Aircraft> Aircraft { get; set; }
        public Guid tenantId => _tenant.Id;
        private readonly Tenant _tenant;
        private readonly Guid _demoMyTenantId;
        private readonly Guid _demoOtherTenantId;
        public TenantedDbContext(DbContextOptions<TenantedDbContext> options,
            IConfiguration config,
            ITenantProvider tenantProvider)
            : base(options)
        {
            _tenant = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _demoMyTenantId = Guid.Parse(config["DemoMyTenantId"]);
            _demoOtherTenantId = Guid.Parse(config["DemoOtherTenantId"]);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Design time connection string for migrations, connection string is overriden later if necessary
            options.UseSqlite(_config.GetConnectionString("ApiDb"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // composite key
            modelBuilder.Entity<Aircraft>().HasKey(nameof(Entities.Aircraft.RegNumber), nameof(Entities.Aircraft.TenantId));
            SeedDatabaseForDemo(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        // EF may have proposed new best practices so just follow those for transactional behavior
        public async Task<int> Commit(CancellationToken cancellationToken = default)
            => await SaveChangesAsync();

        private void SeedDatabaseForDemo(ModelBuilder modelBuilder)
        {
            var tenant1Ac = Entities.Aircraft.GlobalAircraft("N772GK", "N772GK.jpg", "Cessna 172S", false);
            var tenant2Ac = Entities.Aircraft.GlobalAircraft("N5342K", "N5342K.jpg", "Piper Archer", true);

            modelBuilder.Entity<Aircraft>().HasData(tenant1Ac, tenant2Ac);
        }
    }
}