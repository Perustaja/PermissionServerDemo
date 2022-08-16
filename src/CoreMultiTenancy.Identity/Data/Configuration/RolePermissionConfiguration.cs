using System;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        private readonly Guid _defaultAdminRoleId;
        private readonly Guid _defaultNewUserRoleId;

        public RolePermissionConfiguration(Guid defaultAdminRoleId, Guid defaultNewUserRoleId)
        {
            _defaultAdminRoleId = defaultAdminRoleId;
            _defaultNewUserRoleId = defaultNewUserRoleId;
        }

        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // Join table configuration
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            builder
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
            builder
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            SeedGlobalRolePerms(builder);
        }

        /// <summary>
        /// Adds default permissions for global roles.
        /// </summary>
        public EntityTypeBuilder<RolePermission> SeedGlobalRolePerms(EntityTypeBuilder<RolePermission> builder)
        {
            // Admin has all, user is basically readonly
            builder.HasData(
                new RolePermission(_defaultAdminRoleId, PermissionEnum.All),
                new RolePermission(_defaultNewUserRoleId, PermissionEnum.Default)
            );
            return builder;
        }
    }
}