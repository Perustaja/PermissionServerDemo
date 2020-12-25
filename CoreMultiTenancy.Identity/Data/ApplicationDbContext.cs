using System;
using System.Collections.Generic;
using CoreMultiTenancy.Identity.Authorization;
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

        /// <summary>
        /// Seeds permission data from PermissionEnum with associated values.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public ModelBuilder SeedPermissionsData(ModelBuilder builder)
        {
            var permissions = new List<Permission>();
            foreach (PermissionEnum p in Enum.GetValues(typeof(PermissionEnum))
            {
                permissions.Add(new Permission(p, "", "", null));
            }
            builder.Entity<Permission>().HasData(
            // Get each permission enum field

            // Populate with its data via reflection
            )
        }
    }
}