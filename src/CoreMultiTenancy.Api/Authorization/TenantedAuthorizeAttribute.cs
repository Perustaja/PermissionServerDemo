using System;
using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Api.Authorization
{
    /// <summary>
    /// Actions marked with this attribute must have a kvp in RouteData that has a key matching
    /// the identifier specified in appsettings.json
    /// </summary>
    public class TenantedAuthorizeAttribute : TypeFilterAttribute
    {
        public TenantedAuthorizeAttribute(string permissions = "") : base(typeof(TenantedAuthorizeFilter))
            => Arguments = new object[] { permissions };
    }
}