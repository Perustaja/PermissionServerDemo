using PermissionServerDemo.Core.Interfaces;
using PermissionServerDemo.Identity.Data.Configuration;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Extensions;
using PermissionServerDemo.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PermissionServerDemo.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IUnitOfWork
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionCategory> PermissionCategories { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<UserOrganizationRole> UserOrganizationRoles { get; set; }

        private readonly IConfiguration _config;
        private readonly IGlobalRoleProvider _globalRoleProvider;
        private readonly Guid _defaultAdminRoleId;
        private readonly Guid _defaultNewUserRoleId;
        private readonly Guid _aircraftCreateRoleId;
        private readonly Guid _demoShadowAdminId;
        public ApplicationDbContext(IConfiguration config, DbContextOptions<ApplicationDbContext> options,
            IGlobalRoleProvider globalRoleProvider)
            : base(options)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _globalRoleProvider = globalRoleProvider ?? throw new ArgumentNullException(nameof(globalRoleProvider));
            _defaultAdminRoleId = config.GetDemoRoleId("DefaultAdminRoleId");
            _defaultNewUserRoleId = config.GetDemoRoleId("DefaultNewUserRoleId");
            _aircraftCreateRoleId = config.GetDemoRoleId("AircraftCreateRoleId");
            _demoShadowAdminId = Guid.Parse(config["DemoShadowAdminId"]);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(_config.GetConnectionString("IdentityDb"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Permissions seeding MUST be done before others, and in this order to seed properly
            modelBuilder.ApplyConfiguration(new PermissionsSeeder.PermissionCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionsSeeder.PermissionConfiguration());
            // Seed global defaults and setup join tables, this is for old EF core so these may be unnecessary
            modelBuilder.ApplyConfiguration(new RoleConfiguration(_globalRoleProvider));
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(_globalRoleProvider));
            modelBuilder.ApplyConfiguration(new UserOrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new UserOrganizationRoleConfiguration());

            SeedDatabaseForDemo(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        // Note that SaveChangesAsync() works with most providers to rollback by default 
        // on failure. In other words, it's only being exposed so that multiple operations
        // may be performed in a commit by services. No complicated methods are required for this basic
        // transactional behavior. EF may have proposed best practices so just follow those. 
        public async Task<int> Commit(CancellationToken cancellationToken = default)
            => await SaveChangesAsync();

        private void SeedDatabaseForDemo(ModelBuilder modelBuilder)
        {
            // users
            var hasher = new PasswordHasher<User>();
            var shadowAdmin = new User
            {
                Id = _demoShadowAdminId,
                UserName = "shadow@mydomain.com",
                NormalizedUserName = "SHADOW@MYDOMAIN.COM",
                Email = "shadow@mydomain.com",
                NormalizedEmail = "SHADOW@MYDOMAIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = new Guid().ToString(),
            };
            shadowAdmin.PasswordHash = hasher.HashPassword(shadowAdmin, "password");
            modelBuilder.Entity<User>().HasData(shadowAdmin);
        }
    }
}