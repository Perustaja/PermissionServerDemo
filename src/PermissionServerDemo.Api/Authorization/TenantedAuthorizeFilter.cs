using System.Security.Claims;
using Psd.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Core.Tenancy;

namespace PermissionServerDemo.Api.Authorization
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

            var request = new GrpcPermissionAuthorizeRequest()
            {
                UserId = context.HttpContext.User.FindFirstValue("sub"),
                TenantId = tenantId.ToString(),
            };

            if (_permissions != null)
                request.Perms.AddRange(_permissions);

            // Send and set context.Result based on reply
            logger.LogInformation("Authorization request to be sent via GRPC: {Request}", request);
            var reply = await client.AuthorizeAsync(request);
            SetContextResultOnReply(context, reply);
        }

        private void SetContextResultOnReply(AuthorizationFilterContext context, GrpcAuthorizeDecision reply)
        {
            var logger = GetLogger(context.HttpContext);
            logger.LogInformation("Remote authorization result: {Reply}", reply);
            if (!reply.Allowed)
            {
                switch (reply.FailureReason)
                {
                    case (failureReason.Permissionformat):
                        logger.LogCritical("Identity server unable to parse permissions from attribute. {FailureMessage}, {Permissions}", reply.FailureMessage, _permissions);
                        context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                        break;
                    case (failureReason.Tenantnotfound):
                        context.Result = new NotFoundResult(); break;
                    default:
                        context.Result = new ForbidResult(); break;
                }
            }
        }

        // It seems like there is still no easy way to use DI with action filters in .NET Core 6.0,
        // figuring out how to do this in a more normal way would be nice but is low prio
        private GrpcPermissionAuthorize.GrpcPermissionAuthorizeClient GetGrpcClient(HttpContext context)
        {
            return context.RequestServices
                .GetRequiredService(typeof(GrpcPermissionAuthorize.GrpcPermissionAuthorizeClient))
                as GrpcPermissionAuthorize.GrpcPermissionAuthorizeClient;
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