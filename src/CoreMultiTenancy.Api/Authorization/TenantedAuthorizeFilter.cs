using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoreMultiTenancy.Core.Authorization;
using System.Linq;

namespace CoreMultiTenancy.Api.Authorization
{
    public class TenantedAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _permissions;
        public TenantedAuthorizeFilter(PermissionEnum[] permissions)
        {
            _permissions = permissions.Select(p => p.ToString()).ToArray<string>();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // If user is somehow is an invalid state, challenge
            if (context.HttpContext.User?.Identity.IsAuthenticated == false)
            {
                context.Result = new ChallengeResult();
                return;
            }

            // Retrieve client and tenantId from DI
            var client = GetGrpcClient(context.HttpContext);
            var tenantId = GetTenantProvider(context.HttpContext).GetCurrentRequestTenant().Id;

            var request = new PermissionAuthorizeRequest()
            {
                UserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                TenantId = tenantId,
            };
            if (_permissions != null)
                request.Perms.AddRange(_permissions);

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
                        logger.LogCritical($"Identity server unable to parse permissions from attribute. {reply.FailureMessage}, {_permissions}");
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
                .GetRequiredService(typeof(PermissionAuthorize.PermissionAuthorizeClient))
                as PermissionAuthorize.PermissionAuthorizeClient;
        }

        private ITenantProvider GetTenantProvider(HttpContext context)
        {
            return context.RequestServices
                .GetRequiredService(typeof(ITenantProvider))
                as ITenantProvider;
        }

        private ILogger<TenantedAuthorizeFilter> GetLogger(HttpContext context)
        {
            return context.RequestServices
                .GetRequiredService(typeof(ILogger<TenantedAuthorizeFilter>))
                as ILogger<TenantedAuthorizeFilter>;
        }
    }
}