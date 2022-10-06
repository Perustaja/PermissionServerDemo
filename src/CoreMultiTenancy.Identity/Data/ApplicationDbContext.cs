using CoreMultiTenancy.Core.Interfaces;
using CoreMultiTenancy.Identity.Data.Configuration;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        private readonly IConfiguration _config;
        private readonly IGlobalRoleProvider _globalRoleProvider;
        private readonly Guid _defaultAdminRoleId;
        private readonly Guid _defaultNewUserRoleId;
        private readonly Guid _aircraftCreateRoleId;
        private readonly Guid _demoAdminId;
        private readonly Guid _demoShadowAdminId;
        private readonly Guid _demoMyTenantId;
        private readonly Guid _demoOtherTenantId;
        public ApplicationDbContext(IConfiguration config, DbContextOptions<ApplicationDbContext> options,
            IGlobalRoleProvider globalRoleProvider)
            : base(options)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _globalRoleProvider = globalRoleProvider ?? throw new ArgumentNullException(nameof(globalRoleProvider));
            _defaultAdminRoleId = config.GetDemoRoleId("DefaultAdminRoleId");
            _defaultNewUserRoleId = config.GetDemoRoleId("DefaultNewUserRoleId");
            _aircraftCreateRoleId = config.GetDemoRoleId("AircraftCreateRoleId");
            _demoAdminId = Guid.Parse(config["DemoAdminId"]);
            _demoShadowAdminId = Guid.Parse(config["DemoShadowAdminId"]);
            _demoMyTenantId = Guid.Parse(config["DemoMyTenantId"]);
            _demoOtherTenantId = Guid.Parse(config["DemoOtherTenantId"]);
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
            var admin = new User
            {
                Id = _demoAdminId,
                UserName = "admin@mydomain.com",
                NormalizedUserName = "ADMIN@MYDOMAIN.COM",
                Email = "admin@mydomain.com",
                NormalizedEmail = "ADMIN@MYDOMAIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = new Guid().ToString(),
            };
            admin.PasswordHash = hasher.HashPassword(admin, "password");
            admin.UpdateName("Admin", "Admin");

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

            // tenants
            var myOrg = new Organization(_demoMyTenantId, "MyCompany", false, _demoAdminId, "tenantlogo1.jpg");
            var otherOrg = new Organization(_demoOtherTenantId, "OtherCompany", false, _demoShadowAdminId, "tenantlogo2.jpg");

            // tenancy and permissions
            var tenancy1 = new UserOrganization(_demoAdminId, _demoMyTenantId);
            tenancy1.Approve();
            var tenancy2 = new UserOrganization(_demoAdminId, _demoOtherTenantId);
            tenancy2.Approve();
            var shadow = new UserOrganization(_demoShadowAdminId, _demoOtherTenantId);
            var adminRole1 = new UserOrganizationRole(_demoAdminId, _demoMyTenantId, _defaultAdminRoleId);
            var adminRole2 = new UserOrganizationRole(_demoAdminId, _demoOtherTenantId, _defaultAdminRoleId);
            var adminAircraftRole1 = new UserOrganizationRole(_demoAdminId, _demoMyTenantId, _aircraftCreateRoleId);
            var adminAircraftRole2 = new UserOrganizationRole(_demoAdminId, _demoOtherTenantId, _aircraftCreateRoleId);
            var shadowAdminRole = new UserOrganizationRole(_demoShadowAdminId, _demoOtherTenantId, _defaultAdminRoleId);

            // note that global roles are seeded in RoleConfiguration.cs as that would take place
            // outside of a demo environment
            modelBuilder.Entity<User>().HasData(admin, shadowAdmin);
            modelBuilder.Entity<Organization>().HasData(myOrg, otherOrg);
            modelBuilder.Entity<UserOrganization>().HasData(tenancy1, tenancy2);
            modelBuilder.Entity<UserOrganizationRole>().HasData(
                adminRole1, adminRole2, adminAircraftRole1, adminAircraftRole2, shadowAdminRole);
        }
    }
}