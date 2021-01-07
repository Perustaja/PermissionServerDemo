using System;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Tenancy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreMultiTenancy.Api.Authorization
{
    /// <summary>
    /// Actions marked with this attribute must have route data with key "tid" containing the id
    /// of the tenant. Fails if the current User does not have access to the 
    /// </summary>
    public class TenantedAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;
        public TenantedAuthorizeFilter(params string[] permissions)
        {
            _permissions = permissions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Retrieve client from DI
            var client = context.HttpContext.RequestServices
                .GetService(typeof(PermissionAuthorize.PermissionAuthorizeClient)) 
                as PermissionAuthorize.PermissionAuthorizeClient 
                ?? throw new ArgumentNullException("Unable to source gRPC client.");

            var tidOpt = context.HttpContext.GetTenantIdFromRouteData();
            if (tidOpt.IsNone())
                throw new Exception("Action marked with TenantedAuthorizeAttribute did not have necessary RouteData.");

            // Send gRPC message to evaluate authorization
            var request = new PermissionAuthorizeRequest() { TenantId = tidOpt.Unwrap() };
            request.Perms.AddRange(_permissions);
            var reply = await client.AuthorizeAsync(request);

            // Message has more info to use, but just test this now
            if (!reply.Allowed)
                context.Result = new UnauthorizedResult();
        }
    }
}