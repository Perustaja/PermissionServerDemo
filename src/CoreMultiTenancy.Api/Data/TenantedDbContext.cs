using System;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Data
{
    public class TenantedDbContext : DbContext, ITenantedDbContext
    {
        DbSet<Aircraft> Aircraft { get; set; }

        public string TenantModelCacheKey => _tenant.Id;
        private readonly IConfiguration _config;
        private readonly Tenant _tenant;

        /// <summary>
        /// Only used for design time factory.
        /// </summary>
        public TenantedDbContext(DbContextOptions<TenantedDbContext> options) : base(options) { }

        public TenantedDbContext(DbContextOptions<TenantedDbContext> options,
            IConfiguration config,
            ITenantProvider tenantProvider)
            : base(options)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _tenant = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
        }

        /// <summary>
        /// Handles creation of databases for new tenants, and assigns connection string dynamically
        /// for each tenant. Also replaces the ModelCacheKeyFactory for caching of different database results.
        /// </summary>
        protected async override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Bypass tenant configuration if this context was created at design time
            if (!optionsBuilder.IsConfigured)
            {
                // The tenant string is valid because by this point it has been vouched for by
                // the idp, so if it doesn't exist we need to create a new database
                if (!await Database.GetService<IRelationalDatabaseCreator>().ExistsAsync())
                    await Database.MigrateAsync();
                
                optionsBuilder.UseMySql(_tenant.ConnectionString)
                    .ReplaceService<IModelCacheKeyFactory, TenantedModelCacheKeyFactory>();
            }
        }
    }
}