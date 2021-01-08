using System;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CoreMultiTenancy.Api.Data
{
    public class TenantedDbContext : DbContext, ITenantedDbContext
    {
        DbSet<Aircraft> Aircraft { get; set; }

        public string TenantModelCacheKey => _tenant.Id;
        private readonly Tenant _tenant;

        public TenantedDbContext(DbContextOptions<TenantedDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenant = tenantProvider?.GetCurrentRequestTenant() ?? throw new ArgumentNullException(nameof(tenantProvider));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_tenant.ConnectionString)
                .ReplaceService<IModelCacheKeyFactory, TenantedModelCacheKeyFactory>();
        }
    }
}