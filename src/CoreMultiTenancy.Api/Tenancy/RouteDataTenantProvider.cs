using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Tenancy
{
    public class RouteDataTenantProvider : ITenantProvider
    {
        private readonly string _identifier;
        private readonly ILogger<RouteDataTenantProvider> _logger;
        private readonly HttpContext _httpContext;

        public RouteDataTenantProvider(ILogger<RouteDataTenantProvider> logger,
            IConfiguration config,
            IHttpContextAccessor contextAccessor)
        {
            _identifier = config["TenantIdentifiers:RouteData"] ?? throw new ArgumentNullException("Unable to source route data tenant identifier.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContext = contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(contextAccessor.HttpContext));
        }

        public Tenant GetCurrentRequestTenant()
        {
            if (_httpContext == null)
                throw new ArgumentNullException(nameof(_httpContext));

            _httpContext.Request.RouteValues.TryGetValue(_identifier, out object value);
            string s = value.ToString();
            if (String.IsNullOrWhiteSpace(s))
                throw new TenantNotFoundException(_httpContext, _identifier);
            return new Tenant(s);
        }
    }
}