using System;
using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Api.Authorization
{
    /// <summary>
    /// Actions marked with this attribute must have route data with key "tenantId" containing the id
    /// of the tenant.
    /// </summary>
    public class TenantedAuthorizeAttribute : TypeFilterAttribute
    {
        public TenantedAuthorizeAttribute(string permissions = "") : base(typeof(TenantedAuthorizeFilter))
            => Arguments = new object[] { permissions };
    }
}