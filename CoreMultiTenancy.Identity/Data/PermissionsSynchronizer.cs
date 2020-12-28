using System;
using System.Collections.Generic;
using CoreMultiTenancy.Identity.Attributes;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Entities;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace CoreMultiTenancy.Identity.Data
{
    public static class PermissionsSynchronizer
    {
        /// <summary>
        /// Updates or Adds records for all PermissionEnums and PermissionCategoryEnums to database.
        /// </summary>
        public static void SynchronizePermissions(this ApplicationDbContext dbContext)
        {
            // Create PermissionCategory objects, and then add or update them on startup.
            var categories = new HashSet<PermissionCategory>();
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

                if (!categories.Add(pc))
                    throw new Exception("PermissionCategoryEnum seeding failed, multiple enums with same underlying value.");
            }

            var perms = new HashSet<Permission>();
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

                if (!perms.Add(p))
                    throw new Exception("PermissionEnum seeding failed, multiple enums with same underlying value.");
            }
            // Now insert or update values
            foreach (var c in categories)
                InsertOrUpdateCategory(dbContext, c);
            foreach (var p in perms)
                InsertOrUpdatePermission(dbContext, p);
            dbContext.SaveChanges();
        }

        private static IEnumerable<Attribute> GetCustomAttributes(Type t, string member)
        {
            return t
                .GetMember(member)
                .FirstOrDefault(m => m.DeclaringType == t)
                .GetCustomAttributes();
        }

        private static void InsertOrUpdatePermission(ApplicationDbContext context, Permission p)
        {
            var existing = context.Permissions.Find(p.Id);
            if (existing == null)
                context.Add(p);
            else
                context.Entry(existing).CurrentValues.SetValues(p);
        }

        private static void InsertOrUpdateCategory(ApplicationDbContext context, PermissionCategory c)
        {
            var existing = context.PermissionCategories.Find(c.Id);
            if (existing == null)
                context.Add(c);
            else
                context.Entry(existing).CurrentValues.SetValues(c);
        }
    }
}