using System;
using CoreMultiTenancy.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Api.Tenancy
{
    public class RouteDataTenantProvider : ITenantProvider
    {
        private readonly string _routeDataIdentifier;
        private readonly HttpContext _httpContext;

        public RouteDataTenantProvider(IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _routeDataIdentifier = config["TenantIdentifiers:RouteData"] ?? throw new ArgumentNullException("Unable to source route data tenant identifier.");
            _httpContext = contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(contextAccessor.HttpContext));
        }

        public Tenant GetCurrentRequestTenant()
        {
            _httpContext.Request.RouteValues.TryGetValue(_routeDataIdentifier, out object value);
            if (value == null)
                throw new TenantNotFoundException(_httpContext, _routeDataIdentifier);
            return new Tenant(new Guid(value.ToString()));
        }
    }
}