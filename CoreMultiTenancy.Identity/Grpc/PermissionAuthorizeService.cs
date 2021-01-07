using System;
using System.Threading.Tasks;
using Cmt.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Grpc
{
    public class PermissionAuthorizeService : PermissionAuthorize.PermissionAuthorizeBase
    {
        private readonly ILogger<PermissionAuthorizeService> _logger;
        public PermissionAuthorizeService(ILogger<PermissionAuthorizeService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns whether the current user has the given permission(s) within the scope of the specified tenant.
        /// </summary>
        public override async Task<AuthorizeDecision> Authorize(PermissionAuthorizeRequest request, ServerCallContext ctx)
        {
            return await Task.FromResult(new AuthorizeDecision { Allowed = true, FailureMessage = "Hello world!" });
        }
    }
}