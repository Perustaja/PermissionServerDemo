using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Tenancy
{
    public class RouteDataTenantProvider : ITenantProvider
    {
        private readonly string _identifier;
        private readonly HttpContext _httpContext;

        public RouteDataTenantProvider(IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _identifier = config["TenantIdentifiers:RouteData"] ?? throw new ArgumentNullException("Unable to source route data tenant identifier.");
            _httpContext = contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(contextAccessor.HttpContext));
        }

        public Tenant GetCurrentRequestTenant()
        {
            _httpContext.Request.RouteValues.TryGetValue(_identifier, out object value);
            string s = value.ToString();
            if (String.IsNullOrWhiteSpace(s))
                throw new TenantNotFoundException(_httpContext, _identifier);
            return new Tenant(s);
        }
    }
}