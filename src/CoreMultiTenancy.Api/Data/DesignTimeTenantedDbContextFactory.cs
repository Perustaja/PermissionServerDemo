using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreMultiTenancy.Api.Data
{
    /// <summary>
    /// Provides design-time context so that migrations can be applied without making the actual context
    /// handle potential null tenant data.
    /// </summary>
    public class DesignTimeTenantedDbContextFactory : IDesignTimeDbContextFactory<TenantedDbContext>
    {
        public TenantedDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .Build();
            
            var dummyId = config["DummyTenantId"] ?? throw new ArgumentNullException("Unable to source dummy tenant id from config.");
            var o = new DbContextOptionsBuilder<TenantedDbContext>();
            o.UseSqlite(config.GetConnectionString("ApiDb"));
            var dummyProvider = new ManualTenantProvider(new Guid(dummyId));
            return new TenantedDbContext(o.Options, config, dummyProvider);
        }
    }
}