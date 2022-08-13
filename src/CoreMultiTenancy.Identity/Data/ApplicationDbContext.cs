using System;
using System.Threading;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Configuration;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IUnitOfWork
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionCategory> PermissionCategories { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<UserOrganizationRole> UserOrganizationRoles { get; set; }
        private readonly Guid _defaultAdminRoleId;

        public ApplicationDbContext(IConfiguration config, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _defaultAdminRoleId = config.GetDefaultAdminRoleId();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Permissions seeding MUST be done before others, and in this order to seed properly
            builder.ApplyConfiguration(new PermissionsSeeder.PermissionCategoryConfiguration());
            builder.ApplyConfiguration(new PermissionsSeeder.PermissionConfiguration());
            // Pass default role id for seed data
            builder.ApplyConfiguration(new RoleConfiguration(_defaultAdminRoleId));
            builder.ApplyConfiguration(new RolePermissionConfiguration());
            builder.ApplyConfiguration(new UserOrganizationConfiguration());
            builder.ApplyConfiguration(new UserOrganizationRoleConfiguration());

            base.OnModelCreating(builder);
        }

        // Note that SaveChangesAsync() works with most providers to rollback by default 
        // on failure. In other words, it's only being exposed so that multiple operations
        // may be performed in a commit by services. No complicated methods are required for this basic
        // transactional behavior.
        public async Task<int> Commit(CancellationToken cancellationToken = default)
            => await SaveChangesAsync();
    }
}