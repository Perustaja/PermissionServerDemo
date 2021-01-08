using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Tenancy
{
    public class RouteDataTenantProvider : ITenantProvider
    {
        const string _identifier = "tenantId";
        private readonly ILogger<RouteDataTenantProvider> _logger;
        private readonly HttpContext _httpContext;

        public RouteDataTenantProvider(ILogger<RouteDataTenantProvider> logger,
            IHttpContextAccessor contextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContext = contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(contextAccessor));
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