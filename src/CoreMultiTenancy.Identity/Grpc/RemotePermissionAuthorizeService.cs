using Cmt.Protobuf;
using CoreMultiTenancy.Identity.Authorization;
using CoreMultiTenancy.Identity.Interfaces;
using Grpc.Core;

namespace CoreMultiTenancy.Identity.Grpc
{
    public class RemotePermissionAuthorizeService : GrpcPermissionAuthorize.GrpcPermissionAuthorizeBase
    {
        private readonly ILogger<RemotePermissionAuthorizeService> _logger;
        private readonly IAuthorizationEvaluator _authEvaluator;

        public RemotePermissionAuthorizeService(ILogger<RemotePermissionAuthorizeService> logger,
            IAuthorizationEvaluator authEvaluator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authEvaluator = authEvaluator ?? throw new ArgumentNullException(nameof(authEvaluator));
        }

        /// <summary>
        /// Returns whether the current user has the given permission(s) within the scope of the specified tenant.
        /// </summary>
        public override async Task<GrpcAuthorizeDecision> Authorize(GrpcPermissionAuthorizeRequest request, ServerCallContext ctx)
        {
            _logger.LogInformation($"GRPC remote authorization request started. {request}");
            var decision = await _authEvaluator.EvaluateAsync(request.UserId, request.TenantId, 
                request.Perms.ToArray());
            
            var reply = new GrpcAuthorizeDecision()
            {
                Allowed = decision.Allowed,
                FailureMessage = decision.FailureMessage
            };
            if (decision.FailureReason != null)
                reply.FailureReason = MapFailureReason(decision.FailureReason.Value);

            _logger.LogInformation($"GRPC remote authorization request finished, returning reply. {reply}");
            return reply;
        }

        private Cmt.Protobuf.failureReason MapFailureReason(AuthorizeFailureReason reason)
        {
            switch (reason)
            {
                case (AuthorizeFailureReason.PermissionFormat):
                    return Cmt.Protobuf.failureReason.Permissionformat;
                case (AuthorizeFailureReason.TenantNotFound):
                    return Cmt.Protobuf.failureReason.Tenantnotfound;
                case (AuthorizeFailureReason.Unauthorized):
                    return Cmt.Protobuf.failureReason.Unauthorized;
                default:
                    throw new Exception($"Error mapping {reason} to protobuf failureReason");
            }
        }
    }
}