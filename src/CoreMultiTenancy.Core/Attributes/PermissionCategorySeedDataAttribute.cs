using System;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Core.Attributes
{
    /// <summary>
    /// The enum field decorated with this attribute will have its name and description seeded into the db
    /// along with its associated primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PermissionCategorySeedDataAttribute : Attribute
    {
        public string Name { get; set; }
        public bool VisibleToUser { get; set; }

        /// <summary>
        /// Creates an enum mapping profile so the db and application definition stay in synch.
        /// </summary>
        /// <param name="name">
        /// The name to display to the end user in forms
        /// </param>
        /// <param name="visibleToUser">
        /// Whether or not the category and its permissions should be shown to the end user (e.g. no for default category)
        /// </param>
        public PermissionCategorySeedDataAttribute(string name = "", bool visibleToUser = true)
        {
            Name = name;
            VisibleToUser = visibleToUser;
        }
    }
}