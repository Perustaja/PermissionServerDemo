using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using CoreMultiTenancy.Identity.Attributes;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreMultiTenancy.Identity.Data.Configuration
{
    public class PermissionsSeeder
    {
        /// <summary>
        /// Synchronizes PermissionCategories in database from cs file. Must be called before Permissions config.
        /// </summary>
        public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
        {
            public void Configure(EntityTypeBuilder<Permission> builder)
                => SeedGlobalPerms(builder);
        }

        /// <summary>
        /// Synchronizes Permissions in the database with the underlying enum value using its attribute data.
        /// </summary>
        public class PermissionCategoryConfiguration : IEntityTypeConfiguration<PermissionCategory>
        {
            public void Configure(EntityTypeBuilder<PermissionCategory> builder)
                => SeedGlobalPermCats(builder);
        }

        private static EntityTypeBuilder<Permission> SeedGlobalPerms(EntityTypeBuilder<Permission> builder)
        {
            var permsDict = new Dictionary<PermissionEnum, Permission>();
            foreach (PermissionEnum e in Enum.GetValues(typeof(PermissionEnum)))
            {
                var p = new Permission(e);
                var attribs = GetCustomAttributes(typeof(PermissionEnum), e.ToString());

                p.IsObsolete = attribs.Any(a => a is ObsoleteAttribute);
                try
                {
                    var seedData = attribs.OfType<PermissionSeedDataAttribute>().First();
                    p.Name = seedData.Name;
                    p.Description = seedData.Description;
                    p.PermCategoryId = seedData.PermissionCategory;
                    p.VisibleToUser = seedData.VisibleToUser;
                }
                catch
                {
                    throw new Exception($"PermissionEnum {e} did not have required PermissionSeedDataAttribute.");
                }

                if (!permsDict.TryAdd(p.Id, p))
                    throw new Exception("PermissionEnum seeding failed, multiple enums with same underlying value.");
            }
            foreach (var perm in permsDict.Values)
                builder.HasData(perm);
            return builder;
        }

        private static EntityTypeBuilder<PermissionCategory> SeedGlobalPermCats(EntityTypeBuilder<PermissionCategory> builder)
        {
            // Create PermissionCategory objects, and then add or update them on startup.
            var categoriesDict = new Dictionary<PermissionCategoryEnum, PermissionCategory>();
            foreach (PermissionCategoryEnum e in Enum.GetValues(typeof(PermissionCategoryEnum)))
            {
                var pc = new PermissionCategory(e);
                var attribs = GetCustomAttributes(typeof(PermissionCategoryEnum), e.ToString());

                pc.IsObsolete = attribs.Any(a => a is ObsoleteAttribute);
                try
                {
                    pc.Name = attribs.OfType<DisplayAttribute>().First().Name;
                }
                catch
                {
                    throw new Exception($"PermissionCategoryEnum {e} did not have required DisplayAttribute.");
                }

                if (!categoriesDict.TryAdd(pc.Id, pc))
                    throw new Exception("PermissionCategoryEnum seeding failed, multiple enums with same underlying value.");
            }
            foreach (var cat in categoriesDict.Values)
                builder.HasData(cat);
            return builder;
        }

        private static IEnumerable<Attribute> GetCustomAttributes(Type t, string member)
        {
            return t
                .GetMember(member)
                .FirstOrDefault(m => m.DeclaringType == t)
                .GetCustomAttributes();
        }
    }
}