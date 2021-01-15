using System;
using CoreMultiTenancy.Api.Interfaces;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Data
{
    public class RuntimeDbContextFactory<TContext> :  IRuntimeDbContextFactory<TContext> 
        where TContext : DbContext, ITenantedDbContext
    {
        private readonly IServiceProvider _svcProvider;

        public RuntimeDbContextFactory(IServiceProvider svcProvider)
        {
            _svcProvider = svcProvider ?? throw new ArgumentNullException(nameof(svcProvider));
        }

        public TContext CreateContext()
        {
            var logger = _svcProvider.GetService<ILogger<RuntimeDbContextFactory<TContext>>>();
            var db = ActivatorUtilities.CreateInstance<TContext>(_svcProvider);
            if (!db.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                logger.LogInformation($"Initializing tenant {db.TenantId}'s database.");
                db.Database.Migrate();
                logger.LogInformation($"Done initializing tenant {db.TenantId}'s database.");
            }
            return db;
        }
    }
}