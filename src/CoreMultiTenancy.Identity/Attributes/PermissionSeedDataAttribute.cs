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
        public string Name { get; set; }
        public string Description { get; set; }
        public bool VisibleToUser { get; set; }
        public PermissionCategoryEnum PermissionCategory { get; set; }

        /// <summary>
        /// Creates an enum mapping profile so the db and application definition stay in synch.
        /// </summary>
        /// <param name="visibleToUser">
        /// Whether the user is allowed to select this as a permission when creating roles
        /// </param>
        public PermissionSeedDataAttribute(string name, PermissionCategoryEnum cat, string description = "", 
            bool visibleToUser = true)
        {
            Name = name;
            PermissionCategory = cat;
            Description = description;
        }
    }
}