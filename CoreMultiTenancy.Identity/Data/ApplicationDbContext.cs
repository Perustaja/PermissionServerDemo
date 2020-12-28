using System;
using System.Collections.Generic;
using CoreMultiTenancy.Identity.Attributes;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Data.Configuration;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations;

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
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new RolePermissionConfiguration());
            builder.ApplyConfiguration(new UserOrganizationConfiguration());
            builder.ApplyConfiguration(new UserOrganizationRoleConfiguration());

            base.OnModelCreating(builder);
        }
    }
}