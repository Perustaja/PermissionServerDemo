using System.Threading.Tasks;
using Grpc.Core;
using Cmt.Protobuf;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;

namespace CoreMultiTenancy.Identity.Grpc.Aircraft
{
    public class CreateAircraftService : CreateAircraft.CreateAircraftBase
    {
        private readonly IAuthorizationService _authSvc;
        private readonly HttpContext _httpContext;
        public CreateAircraftService(IAuthorizationService authSvc, IHttpContextAccessor httpCtxAccessor)
        {
            _authSvc = authSvc ?? throw new ArgumentNullException(nameof(authSvc));
            _httpContext = httpCtxAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpCtxAccessor));
        }

        public override async Task<AuthorizeDecision> CreateAircraft(CreateAircraftRequest req, ServerCallContext ctx)
        {
            var result = await _authSvc.AuthorizeAsync(_httpContext.User, req, nameof(CreateAircraftRequest));
            return new AuthorizeDecision() { Allowed = result.Succeeded };
        }
    }
}