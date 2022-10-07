using Microsoft.AspNetCore.Mvc;
using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Identity.Authorization;

namespace PermissionServerDemo.Identity.Attributes
{
    /// <summary>
    /// Actions marked with this attribute must have a kvp in RouteData that has a key matching
    /// the identifier specified in appsettings.json
    /// </summary>
    public class TenantedAuthorizeAttribute : TypeFilterAttribute
    {
        public TenantedAuthorizeAttribute(params PermissionEnum[] permissions) : base(typeof(TenantedAuthorizeFilter))
            => Arguments = new object[] { permissions };
    }
}