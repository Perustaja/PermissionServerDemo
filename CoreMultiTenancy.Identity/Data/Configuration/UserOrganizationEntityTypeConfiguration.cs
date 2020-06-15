using CoreMultiTenancy.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class UserOrganizationEntityTypeConfiguration :
    IEntityTypeConfiguration<UserOrganization>
    {
        public void Configure(EntityTypeBuilder<UserOrganization> config)
        {
            // Create many-many join table
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
}