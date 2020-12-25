using System;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Grpc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("0.1")]
    [ApiController]
    [Route("api/v{version:apiVersion}/org/{tid}/[controller]")]
    public class AircraftController : ControllerBase
    {
        private readonly ILogger<AircraftController> _logger;
        private readonly IAuthzChannelService _channelSvc;
        public AircraftController(ILogger<AircraftController> logger, IAuthzChannelService channelSvc)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _channelSvc = channelSvc ?? throw new ArgumentNullException(nameof(channelSvc));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string tid)
        {
            // Authz: Check if user has access to this tenant (no specific permission necessary)
            var client = new BaseAuthorize.BaseAuthorizeClient(_channelSvc.CurrentChannel());
            var reply = await client.AuthorizeAsync(new BaseAuthorizeRequest { TenantId = tid });
            if (!reply.Allowed)
                return Unauthorized();

            return Ok(new Aircraft("N12345"));
        }

        [HttpPost]
        public async  Task<IActionResult> Post(string tid, Aircraft aircraft)
        {
            // Authz: Send gRPC call to see if user has access to this tenant and can create aircraft
            var request = new CreateAircraftRequest() { TenantId = tid };
            var client = new CreateAircraft.CreateAircraftClient(_channelSvc.CurrentChannel());
            var reply = await client.CreateAircraftAsync(request);
            if (!reply.Allowed)
                return Unauthorized();
            
            // Validate aircraft
            // Save to db, return errors if necessary
            return CreatedAtAction(nameof(aircraft), new { RegNumber = aircraft.RegNumber }, aircraft);
        }

        [HttpPut]
        public async Task<IActionResult> Put(string tid, Aircraft aircraft)
        {
            // Authz: Send gRPC call to see if user has access to tenant and can edit aircraft
            var request = new EditAircraftRequest() { TenantId = tid , IsGrounded = aircraft.IsGrounded };
            var client = new EditAircraft.EditAircraftClient(_channelSvc.CurrentChannel());
            var reply = await client.EditAircraftAsync(request);
            if (!reply.Allowed)
                return Unauthorized();

            // Todo:
            // Validate aircraft
            // SaveChanges, return error on failure
            return Ok();
        }
    }
}