using System;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Core.Attributes
{
    /// <summary>
    /// The enum field decorated with this attribute will have its name and description seeded into the db
    /// along with its associated primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PermissionCategorySeedDataAttribute : Attribute
    {
        public string Name { get; set; }

        /// <summary>
        /// Creates an enum mapping profile so the db and application definition stay in synch.
        /// </summary>
        /// <param name="name">
        /// The name to display to the end user in forms
        /// </param>
        public PermissionCategorySeedDataAttribute(string name = "")
        {
            Name = name;
        }
    }
}