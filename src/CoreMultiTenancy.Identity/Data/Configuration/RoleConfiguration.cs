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
        /// Adds default global roles.
        /// </summary>
        public EntityTypeBuilder<Role> SeedGlobalRoles(EntityTypeBuilder<Role> builder)
        {
            builder.HasData
            (
                Role.SeededGlobalRole(_defaultAdminRoleId, "Admin", "Default admin role with all permissions."),
                Role.SeededGlobalRole(_defaultNewUserRoleId, "User", "Default role with minimal permissions.")
            );
            return builder;
        }
    }
}