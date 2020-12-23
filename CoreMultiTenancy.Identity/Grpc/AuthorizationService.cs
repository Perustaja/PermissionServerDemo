using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Authorize;

namespace CoreMultiTenancy.Identity.Grpc
{
    public class AuthorizationService : Authorizer.AuthorizerBase
    {
        private readonly ILogger<AuthorizationService> _logger;
        public AuthorizationService(ILogger<AuthorizationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns whether the given user has access to the action within a given tenant's scope.
        /// </summary>
        public override async Task<AuthorizeDecision> Authorize(AuthorizeRequest request, ServerCallContext ctx)
        {
            return await Task.FromResult(new AuthorizeDecision{ Allowed = true, Message = "Hello world!" });
        }
    }
}