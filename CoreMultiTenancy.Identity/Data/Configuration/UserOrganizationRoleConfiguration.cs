using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class UserOrganizationRoleConfiguration :
    IEntityTypeConfiguration<UserOrganizationRole>
    {
        public void Configure(EntityTypeBuilder<UserOrganizationRole> builder)
        {
            builder.HasKey(uor => new { uor.UserId, uor.OrgId, uor.RoleId });
            builder
                .HasOne(uor => uor.User)
                .WithMany(u => u.UserOrganizationRoles)
                .HasForeignKey(uor => uor.UserId);
            builder
                .HasOne(uor => uor.Organization)
                .WithMany(o => o.UserOrganizationRoles)
                .HasForeignKey(uor => uor.UserId);
            builder
                .HasOne(uor => uor.Role)
                .WithMany(r => r.UserOrganizationRoles)
                .HasForeignKey(uor => uor.RoleId);
        }
    }
}