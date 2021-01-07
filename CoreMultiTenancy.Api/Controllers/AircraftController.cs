using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Authorization;
using CoreMultiTenancy.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("0.1")]
    public class AircraftController : ControllerBase
    {
        private readonly ILogger<AircraftController> _logger;
        public AircraftController(ILogger<AircraftController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [TenantedAuthorize]
        [Route("api/v{version:apiVersion}/org/{tid}/[controller]")]
        public IActionResult Get(string tid)
        {
            return Ok(new Aircraft("N12345"));
        }

        [HttpPost]
        [TenantedAuthorize("AircraftCreate")]
        [Route("api/v{version:apiVersion}/org/{tid}/[controller]")]
        public IActionResult Post(string tid, Aircraft aircraft)
        {
            // Validate aircraft
            // Save to db, return errors if necessary
            return CreatedAtAction(nameof(aircraft), new { RegNumber = aircraft.RegNumber }, aircraft);
        }

        [HttpPut]
        [TenantedAuthorize("AircraftEdit")]
        [Route("api/v{version:apiVersion}/org/{tid}/[controller]")]
        public IActionResult Put(string tid, Aircraft aircraft)
        {
            // Todo:
            // Validate aircraft
            // SaveChanges, return error on failure
            return Ok();
        }
    }
}