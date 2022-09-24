using System;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Core.Attributes
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
        /// This attribute sets up metadata to be saved in the db and used throughout the application
        /// </summary>
        /// <param name="name">
        /// The name to display to the end user in forms
        /// </param>
        /// <param name="visibleToUser">
        /// Whether or not the user is allowed to select this as a permission when creating roles
        /// </param>
        public PermissionSeedDataAttribute(PermissionCategoryEnum cat, string name = "", string description = "", 
            bool visibleToUser = true)
        {
            Name = name;
            PermissionCategory = cat;
            Description = description;
            VisibleToUser = visibleToUser;
        }
    }
}