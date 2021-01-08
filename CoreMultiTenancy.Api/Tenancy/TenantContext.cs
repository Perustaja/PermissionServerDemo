using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Api.Tenancy
{
    public class TenantContext : ITenantContext
    {
        private readonly HttpContext _httpContext;
        private readonly ILogger<TenantContext> _logger;

        public TenantContext(IHttpContextAccessor httpAccessor, ILogger<TenantContext> logger)
        {
            _httpContext = httpAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Option<string> GetTenantIdentifier()
            => _httpContext.GetTenantIdFromRouteData();
    }
}