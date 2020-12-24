using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Entities;
using Grpc.Net.Client;
using Authorize;
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
        public AircraftController(ILogger<AircraftController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> Get(string tid)
        {
            // Authz: Check if user has access to this tenant (no specific permission necessary)
            // Check if authorized based on gRPC call
            using var channel = GrpcChannel.ForAddress("https://localhost:5100");
            var client = new Authorizer.AuthorizerClient(channel);

            var reply = await client.AuthorizeAsync(new AuthorizeRequest { ActionName = "GetAircraft", TenantId = tid });
            _logger.LogInformation($"Response: Allowed: {reply.Allowed} Message: {reply.Message}.");

            if (!reply.Allowed)
                return Unauthorized();
            return Ok(new Aircraft("N12345"));
        }

        [HttpPost]
        public IActionResult Post(string tid, Aircraft aircraft)
        {
            // Authz: Send gRPC call to see if user has access to this tenant and can create aircraft

            // Todo:
            // Validate aircraft
            // Save to db, return errors if necessary
            return CreatedAtAction(nameof(aircraft), new { RegNumber = aircraft.RegNumber }, aircraft);
        }

        [HttpPut]
        public IActionResult Put(string tid, Aircraft aircraft)
        {
            // Authz: Send gRPC call to see if user has access to tenant and can edit aircraft
            // This tests authorization that may require conditional form input, in this case if the 
            // aircraft is grounded.

            // Todo:
            // Validate aircraft
            // SaveChanges, return error on failure
            return Ok();
        }
    }
}