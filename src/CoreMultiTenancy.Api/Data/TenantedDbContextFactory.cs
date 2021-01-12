using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Data
{
    /// <summary>
    /// Provides design-time context so that migrations can be applied without making the actual context
    /// handle potential null tenant data.
    /// </summary>
    public class TenantedDbContextFactory : IDesignTimeDbContextFactory<TenantedDbContext>
    {
        public TenantedDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .Build();
            
            // make dummy database
            var o = new DbContextOptionsBuilder<TenantedDbContext>();
            string connStr = config.GetConnectionString("DesignTimeString") 
                ?? throw new ArgumentNullException("Unable to source design time connection string from json file.");
            o.UseMySql(connStr);
            return new TenantedDbContext(o.Options);
        }
    }
}