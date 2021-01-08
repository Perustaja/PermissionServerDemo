using System;
using Microsoft.AspNetCore.Http;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Api.Tenancy
{
    public static class TenancyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Option<string> GetTenantIdFromRouteData(this HttpContext context)
        {
            context.Request.RouteValues.TryGetValue("tenantId", out object value);
            string s = value.ToString();
            if (!String.IsNullOrWhiteSpace(s))
                return Option<string>.Some(s);
            return Option<string>.None;
        }
    }
}