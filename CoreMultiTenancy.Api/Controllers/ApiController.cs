using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Entities;
using Grpc.Net.Client;
using Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        public ApiController(ILogger<ApiController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{tid}/aircraft")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAircraft(string tid)
        {
            // Check if authorized based on gRPC call
            using var channel = GrpcChannel.ForAddress("https://localhost:5100");
            var client = new Authorizer.AuthorizerClient(channel);

            var reply = await client.AuthorizeAsync(new AuthorizeRequest { ActionName = "GetAircraft", TenantId = tid });
            _logger.LogInformation($"Response: Allowed: {reply.Allowed} Message: {reply.Message}.");

            if (!reply.Allowed)
                return Unauthorized();
            return Ok(new Aircraft("N12345"));
        }
    }
}