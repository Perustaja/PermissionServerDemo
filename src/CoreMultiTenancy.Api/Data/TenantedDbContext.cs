using System;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Extensions;
using CoreMultiTenancy.Api.Interfaces;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Data
{
    public class TenantedDbContext : DbContext, ITenantedDbContext
    {
        DbSet<Aircraft> Aircraft { get; set; }
        public string TenantModelCacheKey => _tenant.Id;
        public string TenantId => _tenant.Id;
        private readonly string _connectionString;
        private readonly Tenant _tenant;

        public TenantedDbContext(DbContextOptions<TenantedDbContext> options,
            IConfiguration config,
            ITenantProvider tenantProvider)
            : base(options)
        {
            _tenant = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
            _connectionString = config.GetTenantedConnectionString(_tenant.Id) ?? throw new ArgumentNullException("Unable to source template connection string from config.");
        }

        /// <summary>
        /// Overwrites connection string if request is not a migrations request, and 
        /// replaces the ModelCacheKeyFactory for caching of different database results.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Bypass tenant configuration if this context was created at design time
            if (!optionsBuilder.IsConfigured)
            {
                // The tenant string is valid because by this point it has been vouched via gRPC
                optionsBuilder.UseMySql(_connectionString)
                    .ReplaceService<IModelCacheKeyFactory, TenantedModelCacheKeyFactory>();
            }
        }
    }
}