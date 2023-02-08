using PermissionServerDemo.Api.Entities;
using PermissionServerDemo.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using PermissionServer;

namespace PermissionServerDemo.Api.Data
{
    public class TenantedDbContext : DbContext, IUnitOfWork
    {
        private readonly IConfiguration _config;
        DbSet<Aircraft> Aircraft { get; set; }
        public Guid tenantId { get; }
        private readonly Guid _demoMyTenantId;
        private readonly Guid _demoOtherTenantId;
        public TenantedDbContext(DbContextOptions<TenantedDbContext> options,
            IConfiguration config,
            ITenantProvider tenantProvider)
            : base(options)
        {
            tenantId = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
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
            // tenancy filter
            modelBuilder.Entity<Aircraft>().HasQueryFilter(ac => EF.Property<Guid>(ac, "TenantId") == tenantId);

            SeedDatabaseForDemo(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        // EF may have proposed new best practices so just follow those for transactional behavior
        public async Task<int> Commit(CancellationToken cancellationToken = default)
            => await SaveChangesAsync();

        private void SeedDatabaseForDemo(ModelBuilder modelBuilder)
        {
            var tenant1Ac = new Aircraft("N772GK", _demoMyTenantId, "N772GK.jpg", "Cessna 172S");
            var tenant2Ac = new Aircraft("N5342K", _demoOtherTenantId, "N5342K.jpg", "Piper Archer");

            modelBuilder.Entity<Aircraft>().HasData(tenant1Ac, tenant2Ac);
        }
    }
}