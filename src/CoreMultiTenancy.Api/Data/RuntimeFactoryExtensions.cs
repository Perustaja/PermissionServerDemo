using System;
using CoreMultiTenancy.Api.Interfaces;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoreMultiTenancy.Api.Data
{
    public static class RuntimeFactoryExtensions
    {
        /// <summary>
        /// Registers the RuntimeDbContextFactory with proper scoping, running the builder
        /// on each request. Default lifetime is scoped, so this doesn't work with blazor.
        /// </summary>
        public static IServiceCollection AddRuntimeDbContextFactory<TContext>(
            this IServiceCollection collection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TContext : DbContext, ITenantedDbContext
        {
            // Instantiate with scoped provider
            collection.Add(new ServiceDescriptor(
                typeof(IRuntimeDbContextFactory<TContext>),
                sp => new RuntimeDbContextFactory<TContext>(sp),
                lifetime));

            // Run per request
            collection.Add(new ServiceDescriptor(
                typeof(DbContextOptions<TContext>),
                sp => GetOptions<TContext>(optionsAction, sp),
                lifetime));
            
            return collection;
        }

        /// <returns>The Options for a specific DbContext</returns>
        private static DbContextOptions<TContext> GetOptions<TContext>(
            Action<DbContextOptionsBuilder> action,
            IServiceProvider sp = null) where TContext : DbContext
        {
            var ob = new DbContextOptionsBuilder<TContext>();
            if (sp != null)
                ob.UseApplicationServiceProvider(sp);
            action?.Invoke(ob);
            return ob.Options;
        }
    }
}