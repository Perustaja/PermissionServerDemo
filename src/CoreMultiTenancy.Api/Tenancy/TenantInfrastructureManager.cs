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
    // I've consistently been stumped trying to solve this without the locator antipattern. The tenant id is
    // sourced from the grpc request yet the dbcontext needs it via injection. There's no way the tenant
    // id is known at construction time. I could stick the tenant id in the httpcontext request but
    // then it kind of defeats the purpose of a grpc contract. However, as it stands testing
    // this class is kind of tricky to go about. For now, it works but a more DI friendly way would be
    // ideal.
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