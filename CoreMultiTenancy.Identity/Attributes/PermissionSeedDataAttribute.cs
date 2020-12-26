using System;
using CoreMultiTenancy.Identity.Authorization;

namespace CoreMultiTenancy.Identity.Attributes
{
    /// <summary>
    /// The enum field decorated with this attribute will have its name and description seeded into the db
    /// along with its associated primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PermissionSeedDataAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public PermissionCategoryEnum PermissionCategory { get; private set; }

        /// <summary>
        /// Creates an enum mapping profile so the db and application definition stay in synch.
        /// </summary>
        public PermissionSeedDataAttribute(string name, PermissionCategoryEnum cat, string description = "")
        {
            Name = name;
            PermissionCategory = cat;
            Description = description;
        }
    }
}