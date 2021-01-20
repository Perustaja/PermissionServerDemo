using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Tenancy
{
    // HttpContext at the gRPC may possibly reflect changes, so one way would be to modify some value
    // and access it via an ITenantProvider. This would make this easier to test as opposed to using 
    // ActivatorUtilities here, but since full integration tests are necessary I don't think it's a big deal.
    public class TenantInfrastructureManager<TContext> : ITenantInfrastructureManager<TContext>
        where TContext : DbContext, ITenantedDbContext
    {
        private readonly ILogger<TenantInfrastructureManager<TContext>> _logger;
        private readonly IServiceProvider _svcProvider;

        public TenantInfrastructureManager(ILogger<TenantInfrastructureManager<TContext>> logger,
            IServiceProvider svcProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _svcProvider = svcProvider ?? throw new ArgumentNullException(nameof(svcProvider));
        }
        public async Task<bool> InitializeTenantAsync(string tenantId)
        {
            var manualProvider = new ManualTenantProvider(tenantId);
            var db = ActivatorUtilities.CreateInstance<TContext>(_svcProvider, manualProvider);
            if (!await db.Database.GetService<IRelationalDatabaseCreator>().ExistsAsync())
            {
                try
                {
                    _logger.LogInformation($"Initializing tenant {tenantId}'s database.");
                    await db.Database.MigrateAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unable to create {tenantId}'s database: {e.ToString()}");
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> DeleteTenantAsync(string tenantId)
        {
            var manualProvider = new ManualTenantProvider(tenantId);
            var db = ActivatorUtilities.CreateInstance<TContext>(_svcProvider, manualProvider);
            if (await db.Database.GetService<IRelationalDatabaseCreator>().ExistsAsync())
            {
                try
                {
                    _logger.LogInformation($"Removing unfinished tenant {tenantId}'s database.");
                    await db.Database.EnsureDeletedAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unable to delete unfinished tenant {tenantId}'s database: {e.ToString()}");
                    return false;
                }
            }
            return true;
        }
    }
}