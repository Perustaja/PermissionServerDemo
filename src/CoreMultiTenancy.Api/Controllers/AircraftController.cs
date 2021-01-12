using System;
using CoreMultiTenancy.Api.Authorization;
using CoreMultiTenancy.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AircraftController : ControllerBase
    {
        private readonly ILogger<AircraftController> _logger;
        public AircraftController(ILogger<AircraftController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [TenantedAuthorize]
        public IActionResult Get(string tenantId)
        {
            return Ok(new Aircraft("N12345"));
        }

        [HttpPost]
        [TenantedAuthorize("AircraftCreate")]
        [Route("{tenantId}/[controller]")]
        public IActionResult Post(string tenantId, Aircraft aircraft)
        {
            // Validate aircraft
            // Save to db, return errors if necessary
            return CreatedAtAction(nameof(aircraft), new { RegNumber = aircraft.RegNumber }, aircraft);
        }

        [HttpPut]
        [TenantedAuthorize("AircraftEdit")]
        [Route("{tenantId}/[controller]")]
        public IActionResult Put(string tenantId, Aircraft aircraft)
        {
            // Todo:
            // Validate aircraft
            // SaveChanges, return error on failure
            return Ok();
        }
    }
}