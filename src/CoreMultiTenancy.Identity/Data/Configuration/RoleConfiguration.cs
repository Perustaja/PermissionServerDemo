using System;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        private readonly Guid _defaultAdminRoleId;
        private readonly Guid _defaultNewUserRoleId;

        public RoleConfiguration(Guid defaultAdminRoleId, Guid defaultNewUserRoleId)
        {
            _defaultAdminRoleId = defaultAdminRoleId;
            _defaultNewUserRoleId = defaultNewUserRoleId;

        }
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            SeedGlobalRoles(builder);
        }

        /// <summary>
        /// Adds default global roles for the owner and for new users joining the tenant.
        /// </summary>
        public EntityTypeBuilder<Role> SeedGlobalRoles(EntityTypeBuilder<Role> builder)
        {
            builder.HasData
            (
                new Role(_defaultAdminRoleId, "Admin"),
                new Role(_defaultNewUserRoleId, "User")
            );
            return builder;
        }
    }
}