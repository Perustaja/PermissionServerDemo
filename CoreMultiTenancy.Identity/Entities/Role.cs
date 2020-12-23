using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Entities
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
        [ForeignKey("OrgId")]
        public Organization Organization { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
        public List<UserOrganizationRole> UserOrganizationRoles { get; set; }
        public Role() { }

        /// <summary>
        /// Creates a new role specific to a tenant.
        /// </summary>
        public Role(Guid orgId, string desc)
        {
            IsGlobal = false;
            OrgId = orgId;
            Description = desc;
        }

        /// <summary>
        /// Creates a global role accessible by all tenants.
        /// </summary>
        public Role(string desc)
        {
            IsGlobal = true;
            Description = desc;
        }
    }
}