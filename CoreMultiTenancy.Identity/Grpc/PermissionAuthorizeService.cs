using System;
using System.Linq;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Identity.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Grpc
{
    public class PermissionAuthorizeService : PermissionAuthorize.PermissionAuthorizeBase
    {
        private readonly ILogger<PermissionAuthorizeService> _logger;
        private readonly IRemoteAuthorizationEvaluator _remoteAuthEvaluator;

        public PermissionAuthorizeService(ILogger<PermissionAuthorizeService> logger,
            IRemoteAuthorizationEvaluator remoteAuthEvaluator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _remoteAuthEvaluator = remoteAuthEvaluator ?? throw new ArgumentNullException(nameof(remoteAuthEvaluator));
        }

        /// <summary>
        /// Returns whether the current user has the given permission(s) within the scope of the specified tenant.
        /// </summary>
        public override async Task<AuthorizeDecision> Authorize(PermissionAuthorizeRequest request, ServerCallContext ctx)
            => await _remoteAuthEvaluator.EvaluateAsync(request.UserId, request.TenantId, request.Perms.ToArray());
    }
}