using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PermissionServerDemo.Identity.Entities
{
    /// <summary>
    /// Modified framework role to work with user-defined roles. A role may be specific to a tenant, 
    /// or may be global and shared as a non-removable default value across tenants.
    /// </summary>
    public class Role : IdentityRole<Guid>
    {
        public Guid? OrgId { get; private set; }
        [StringLength(50)]
        public string Description { get; private set; }
        public bool IsGlobal { get; private set; }
        /// <summary>Whether or not the role is the default for a new tenant owner.</summary>
        public bool IsGlobalAdminDefault { get; private set; }
        /// <summary>Whether or not the role is the default for a new user.</summary>
        public bool IsGlobalDefaultForNewUsers { get; private set; }
        /// <summary>Whether or not the role is the default for a new user for this tenant. Takes priority over global.</summary>
        public bool IsTenantDefaultForNewUsers { get; private set; }
        [ForeignKey("OrgId")]
        public Organization Organization { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public List<UserOrganizationRole> UserOrganizationRoles { get; set; }
        public Role() { }
        /// <summary>
        /// Creates a new role specific to a tenant.
        /// </summary>
        public Role(string name, string desc)
        {
            Name = name;
            NormalizedName = name.ToUpper();
            IsGlobal = false;
            Description = desc;
            IsGlobalAdminDefault = false;
            IsGlobalDefaultForNewUsers = false;
            IsTenantDefaultForNewUsers = false;
        }

        /// <summary>
        /// Only used for demo purposes where a hardcoded id is necessary to seed in a user
        /// </summary>
        public static Role SeededGlobalRoleForDemo(Guid id, string name, string desc)
        {
            var r = new Role();
            r.Id = id;
            r.IsGlobal = true;
            r.Name = name;
            r.NormalizedName = name.ToUpper();
            r.Description = desc;
            r.IsGlobalAdminDefault = false;
            r.IsGlobalDefaultForNewUsers = false;
            r.IsTenantDefaultForNewUsers = false;
            return r;
        }

        ///<returns>A global role accessible by all tenants to be tracked by migrations.</returns>
        public static Role SeededGlobalRole(string name, string desc)
        {
            var r = new Role();
            r.IsGlobal = true;
            r.Name = name;
            r.NormalizedName = name.ToUpper();
            r.Description = desc;
            r.IsGlobalAdminDefault = false;
            r.IsGlobalDefaultForNewUsers = false;
            r.IsTenantDefaultForNewUsers = false;
            return r;
        }

        /// <summary>Configures this role to be assigned to all new tenant owners within the application.</summary>
        public void SetAsGlobalAdminRole() => IsGlobalAdminDefault =true;
        public void SetAsTenantDefaultNewUserRole() => IsTenantDefaultForNewUsers = true;
        public void SetAsGlobalDefaultNewUserRole() => IsGlobalDefaultForNewUsers = true;

        /// <summary>
        /// Sets this Role as belonging to the given Organization. Silently fails if this Role is global.
        /// </summary>
        public void SetOrganization(Guid orgId)
        {
            if (!IsGlobal)
                OrgId = orgId;
        }
    }
}