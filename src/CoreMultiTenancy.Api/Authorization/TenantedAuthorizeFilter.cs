using System.Security.Claims;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CoreMultiTenancy.Core.Authorization;

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
            var logger = GetLogger(context.HttpContext);
            logger.LogInformation("Beginning authorization request from Api to Idp.");

            // If user is somehow is an invalid state, challenge
            if (context.HttpContext.User?.Identity.IsAuthenticated == false)
            {
                logger.LogWarning("User was not authenticated for GRPC authorization. Returning challenge.");
                context.Result = new ChallengeResult();
                return;
            }

            // Retrieve client and tenantId from DI
            var client = GetGrpcClient(context.HttpContext);
            var tenantId = GetTenantProvider(context.HttpContext).GetCurrentRequestTenant().Id;

            var request = new PermissionAuthorizeRequest()
            {
                UserId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                TenantId = tenantId.ToString(),
            };

            if (_permissions != null)
                request.Perms.AddRange(_permissions);

            // Send and set context.Result based on reply
            logger.LogInformation($"Authorization request to be sent via GRPC: {request}");
            var reply = await client.AuthorizeAsync(request);
            SetContextResultOnReply(context, reply);
        }

        private void SetContextResultOnReply(AuthorizationFilterContext context, AuthorizeDecision reply)
        {
            var logger = GetLogger(context.HttpContext);
            logger.LogInformation($"Remote authorization result: {reply}");
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