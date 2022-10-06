using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Core.Tenancy;
using CoreMultiTenancy.Identity.Interfaces;

namespace CoreMultiTenancy.Identity.Authorization
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
            var tenantId = GetTenantProvider(context.HttpContext).GetCurrentRequestTenant().Id.ToString();
            var evaulator = GetEvaluator(context.HttpContext);

            logger.LogInformation("Beginning local authorization request.");

            // If user is somehow is an invalid state, challenge
            if (context.HttpContext.User?.Identity.IsAuthenticated == false)
            {
                logger.LogWarning("User was not authenticated for authorization. Returning challenge.");
                context.Result = new ChallengeResult();
                return;
            }
            var userId = context.HttpContext.User.FindFirstValue("sub");

            // Evaluate and set context.Result based on decision
            logger.LogInformation($"Authorizing local request: user {userId}, tenant {tenantId}, perms {(object)_permissions}");
            var decision = await evaulator.EvaluateAsync(userId, tenantId, _permissions);
            SetContextResultOnDecision(context, decision);
        }

        private void SetContextResultOnDecision(AuthorizationFilterContext context, AuthorizeDecision decision)
        {
            var logger = GetLogger(context.HttpContext);
            logger.LogInformation($"Remote authorization result: {decision}");
            if (!decision.Allowed)
            {
                switch (decision.FailureReason)
                {
                    case (AuthorizeFailureReason.PermissionFormat):
                        logger.LogCritical($"Unable to parse permissions from local attribute. {decision.FailureMessage}, {_permissions}");
                        context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                        break;
                    case (AuthorizeFailureReason.TenantNotFound):
                        context.Result = new NotFoundResult(); break;
                    default:
                        context.Result = new ForbidResult(); break;
                }
            }
        }

        // It seems like there is still no easy way to use DI with action filters in .NET Core 6.0,
        // figuring out how to do this in a more normal way would be nice but is low prio
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

        private IAuthorizationEvaluator GetEvaluator(HttpContext context)
        {
            return context.RequestServices
                .GetRequiredService(typeof(IAuthorizationEvaluator))
                as IAuthorizationEvaluator;
        }
    }
}