using System;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
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
            const string adminRoleId = "2301D884-221A-4E7D-B509-0113DCC043E1";
            const string mechRoleId = "7D9B7113-A8F8-4035-99A7-A20DD400F6A3";
            const string pilotRoleId = "78A7570F-3CE5-48BA-9461-80283ED1D94D";
            
            builder.HasData(
                // Admin
                new RolePermission(new Guid(adminRoleId), PermissionEnum.All),
                // Mechanic
                new RolePermission(new Guid(mechRoleId), PermissionEnum.AircraftCreate),
                new RolePermission(new Guid(mechRoleId), PermissionEnum.AircraftEdit),
                new RolePermission(new Guid(mechRoleId), PermissionEnum.AircraftDelete),
                // Pilot
                new RolePermission(new Guid(pilotRoleId), PermissionEnum.AircraftEdit)
            );
            return builder;
        }
    }
}