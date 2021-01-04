using System;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        private readonly Guid _defaultRoleId;

        public RoleConfiguration(Guid defaultRoleId) => _defaultRoleId = defaultRoleId;
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            SeedGlobalRoles(builder);
        }
        
        /// <summary>
        /// Adds default global roles.
        /// </summary>
        public EntityTypeBuilder<Role> SeedGlobalRoles(EntityTypeBuilder<Role> builder)
        {
            const string adminRoleId = "2301D884-221A-4E7D-B509-0113DCC043E1";
            const string mechRoleId = "7D9B7113-A8F8-4035-99A7-A20DD400F6A3";
            Guid pilotRoleId = _defaultRoleId;

            builder.HasData(
                new Role(new Guid(adminRoleId), "Admin"),
                new Role(new Guid(mechRoleId), "Mechanic"),
                new Role(pilotRoleId, "Pilot")
            );
            return builder;
        }
    }
}