using System;
using System.IO;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreMultiTenancy.Api.Data
{
    /// <summary>
    /// Provides design-time context so that migrations can be applied without making the actual context
    /// handle potential null tenant data.
    /// </summary>
    public class DesignTimeTenantedDbContextFactory : IDesignTimeDbContextFactory<TenantedDbContext>
    {
        private readonly IServiceProvider _svcProvider;

        public DesignTimeTenantedDbContextFactory(IServiceProvider svcProvider)
        {
            _svcProvider = svcProvider ?? throw new ArgumentNullException(nameof(svcProvider));
        }
        public TenantedDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .Build();
            
            // make dummy database
            var dummyId = config["DummyTenantId"] ?? throw new ArgumentNullException("Unable to source dummy tenant id from config.");
            var o = new DbContextOptionsBuilder<TenantedDbContext>();
            var dummyProvider = new ManualTenantProvider(dummyId);
            return ActivatorUtilities.CreateInstance<TenantedDbContext>(_svcProvider, dummyProvider);
        }
    }
}