using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Tenancy;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Authorization
{
    public class TenantedAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly List<string> Permissions;
        public TenantedAuthorizeFilter(string permissions)
        {
            Permissions = new List<string>();
            if (!String.IsNullOrWhiteSpace(permissions))
            {
                foreach (string s in permissions?.Split(','))
                    Permissions.Add(s.Trim());
            }
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // All endpoints require authentication, however ensure User is logged in.
            if (context.HttpContext.User?.Identity.IsAuthenticated == false)
                context.Result = new ChallengeResult();

            // Retrieve client from DI
            var client = GetGrpcClient(context.HttpContext);

            var tidOpt = context.HttpContext.GetTenantIdFromRouteData();
            if (tidOpt.IsNone())
                throw new Exception("Action marked with TenantedAuthorizeAttribute did not have necessary RouteData.");

            var request = new PermissionAuthorizeRequest()
            {
                UserId = context.HttpContext.User.FindFirst(JwtClaimTypes.Subject)?.Value,
                TenantId = tidOpt.Unwrap(),
            };
            request.Perms.AddRange(Permissions);

            // Send and set context.Result based on reply
            var reply = await client.AuthorizeAsync(request);
            SetContextResultOnReply(context, reply);
        }

        private void SetContextResultOnReply(AuthorizationFilterContext context, AuthorizeDecision reply)
        {
            var logger = GetLogger(context.HttpContext);
            if (!reply.Allowed)
            {
                switch (reply.FailureReason)
                {
                    case (failureReason.Permissionformat):
                        logger.LogError($"Identity server unable to parse permissions from attribute. {reply.FailureMessage}, {Permissions}");
                        context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                        break;
                    case (failureReason.Tenantnotfound):
                        context.Result = new NotFoundResult(); break;
                    default:
                        context.Result = new UnauthorizedResult(); break;
                }
            }
        }

        // TODO: Figure out how to make DI work with action filters in a non-annoying way to test this
        private PermissionAuthorize.PermissionAuthorizeClient GetGrpcClient(HttpContext context)
        {
            return context.RequestServices
                .GetService(typeof(PermissionAuthorize.PermissionAuthorizeClient))
                as PermissionAuthorize.PermissionAuthorizeClient
                ?? throw new ArgumentNullException("Unable to source gRPC client.");
        }

        private ILogger<TenantedAuthorizeFilter> GetLogger(HttpContext context)
        {
            return context.RequestServices
                .GetService(typeof(ILogger<TenantedAuthorizeFilter>))
                as ILogger<TenantedAuthorizeFilter>
                ?? throw new ArgumentNullException("Unable to source logger.");
        }
    }
}