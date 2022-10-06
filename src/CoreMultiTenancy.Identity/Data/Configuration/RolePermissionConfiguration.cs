using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        private readonly IGlobalRoleProvider _globalRoleProvider;

        public RolePermissionConfiguration(IGlobalRoleProvider globalRoleProvider)
        {
            _globalRoleProvider = globalRoleProvider ?? throw new ArgumentNullException(nameof(globalRoleProvider));
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
            builder.HasData(_globalRoleProvider.GetGlobalRolePermissions());
            return builder;
        }
    }
}