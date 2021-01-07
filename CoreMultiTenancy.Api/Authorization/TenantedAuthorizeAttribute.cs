using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Api.Authorization
{
    public class TenantedAuthorizeAttribute : TypeFilterAttribute
    {
        public TenantedAuthorizeAttribute(params string[] permissions) : base(typeof(TenantedAuthorizeFilter))
            => Arguments = permissions;
    }
}