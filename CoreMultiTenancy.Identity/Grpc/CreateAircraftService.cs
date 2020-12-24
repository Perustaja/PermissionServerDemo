using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Cmt.Protobuf;

namespace CoreMultiTenancy.Identity.Grpc
{
    public class CreateAircraftService : CreateAircraft.CreateAircraftBase
    {
        private readonly ILogger<CreateAircraftService> _logger;
        public CreateAircraftService(ILogger<CreateAircraftService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns whether the given user has access to the action within a given tenant's scope.
        /// </summary>
        public override async Task<AuthorizeDecision> CreateAircraft(BaseAuthorizeRequest request, ServerCallContext ctx)
        {
            return await Task.FromResult(new AuthorizeDecision{ Allowed = true, Message = "Hello world!" });
        }
    }
}