using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    /// <summary>
    /// Many-to-Many join table configuration for UserOrganization.
    /// </summary>
    public class UserOrganizationEntityTypeConfiguration :
    IEntityTypeConfiguration<UserOrganization>
    {
        public void Configure(EntityTypeBuilder<UserOrganization> config)
        {
            config.HasKey(uo => new { uo.UserId, uo.OrganizationId });
            config
                .HasOne(uo => uo.User)
                .WithMany(u => u.UserOrganizations)
                .HasForeignKey(uo => uo.UserId);
            config
                .HasOne(uo => uo.Organization)
                .WithMany(o => o.UserOrganizations)
                .HasForeignKey(uo => uo.OrganizationId);
        }
    }

    /// <summary>
    /// Many-to-Many join table configuration for RolePermission.
    /// </summary>
    public class RolePermissionEntityTypeConfiguration :
    IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> config)
        {
            config.HasKey(rp => new { rp.RoleId, rp.PermissionId });
            config
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
            config
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        }
    }

    /// <summary>
    /// Three-Way join table for User, Organization, and Role.
    /// </summary>
    public class UserOrganizationRoleEntityTypeConfiguration :
    IEntityTypeConfiguration<UserOrganizationRole>
    {
        public void Configure(EntityTypeBuilder<UserOrganizationRole> config)
        {
            config.HasKey(uor => new { uor.UserId, uor.OrgId, uor.RoleId });
            config
                .HasOne(uor => uor.User)
                .WithMany(u => u.UserOrganizationRoles)
                .HasForeignKey(uor => uor.UserId);
            config
                .HasOne(uor => uor.Organization)
                .WithMany(o => o.UserOrganizationRoles)
                .HasForeignKey(uor => uor.UserId);
            config
                .HasOne(uor => uor.Role)
                .WithMany(r => r.UserOrganizationRoles)
                .HasForeignKey(uor => uor.RoleId);
        }
    }
}