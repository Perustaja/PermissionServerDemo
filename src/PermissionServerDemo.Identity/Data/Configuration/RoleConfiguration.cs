using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PermissionServerDemo.Identity.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        private readonly IGlobalRoleProvider _globalRoleProvider;

        public RoleConfiguration(IGlobalRoleProvider globalRoleProvider)
        {
            _globalRoleProvider = globalRoleProvider ?? throw new ArgumentNullException(nameof(globalRoleProvider));
        }
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            SeedGlobalRoles(builder);
        }

        public EntityTypeBuilder<Role> SeedGlobalRoles(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(_globalRoleProvider.GetGlobalRoles());
            return builder;
        }
    }
}