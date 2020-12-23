using System;
using CoreMultiTenancy.Identity.Data.Configuration;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionCategory> PermissionCategories { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<UserOrganizationRole> UserOrganizationRoles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Join tables
            builder.ApplyConfiguration(new UserOrganizationEntityTypeConfiguration());
            builder.ApplyConfiguration(new RolePermissionEntityTypeConfiguration());
            builder.ApplyConfiguration(new UserOrganizationRoleEntityTypeConfiguration());

            base.OnModelCreating(builder);
        }
    }
}