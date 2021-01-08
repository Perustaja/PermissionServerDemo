using System;
using Microsoft.AspNetCore.Http;

namespace CoreMultiTenancy.Api.Tenancy
{
    public class TenantNotFoundException : Exception
    {
        public readonly string _customMessage;
        public override string Message => _customMessage;

        /// <summary>
        /// Represents an exception where the tenant information is sourced there HttpContext RouteData,
        /// and the expected key was not found.
        /// </summary>
        public TenantNotFoundException(HttpContext requestContext, string expectedKey)
        {
            string temp =
            $"Unable to source tenant id from RouteData: Expected key: {expectedKey} Actual kvps: ";
            foreach (var kvp in requestContext.Request.RouteValues)
                temp += (kvp.Key + ":" + kvp.Value);
            _customMessage = temp; 
        }
    }
}