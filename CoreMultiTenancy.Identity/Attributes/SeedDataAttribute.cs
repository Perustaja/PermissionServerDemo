using System;

namespace CoreMultiTenancy.Identity.Attributes
{
    /// <summary>
    /// The enum field decorated with this attribute will have its name and description seeded into the db
    /// along with its associated primary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SeedDataAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// Creates an enum mapping profile so that the primary key and description are mapped regardless
        /// of internal changes to enum name or value.
        /// </summary>
        /// <param name="name">An end-user formatted name.</param>
        /// <param name="description">An end-user formatted description.</param>
        public SeedDataAttribute(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}