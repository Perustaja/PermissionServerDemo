using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoreMultiTenancy.Identity.Data.Configuration.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        private static IServiceCollection AddGlobalRoles(this IServiceCollection sc)
        {
            sc.AddOptions();
            sc.TryAdd(ServiceDescriptor.Transient<IGlobalRoleProvider, DefaultGlobalRoleProvider>());
            return sc;
        }

        /// <summary>
        /// Sets up global roles to be seeded into the database with migrations.
        /// </summary>
        public static IServiceCollection AddGlobalRoles(this IServiceCollection sc, Action<GlobalRolesOptions> config)
        {
            if (sc == null)
                throw new ArgumentNullException(nameof(sc));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            sc.Configure(config);
            return sc.AddGlobalRoles();
        }
    }
}