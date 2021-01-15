using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Authorization;
using CoreMultiTenancy.Api.Data;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AircraftController : ControllerBase
    {
        private readonly IRuntimeDbContextFactory<TenantedDbContext> _dbContextFactory;

        public AircraftController(IRuntimeDbContextFactory<TenantedDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        [HttpGet]
        [TenantedAuthorize]
        [Route("{tenantId}/[controller]")]
        public async Task<IActionResult> Get(string tenantId)
            => Ok(await _dbContextFactory.CreateContext().Set<Aircraft>().ToListAsync());

        // [HttpPost]
        // [TenantedAuthorize("AircraftCreate")]
        // [Route("{tenantId}/[controller]")]
        // public async Task<IActionResult> Post(string tenantId, Aircraft aircraft)
        // {
        //     if (await _dbContext.Set<Aircraft>().AnyAsync(a => a.RegNumber == aircraft.RegNumber))
        //         return Conflict($"Aircraft already exists with registration number: {aircraft.RegNumber}");

        //     _dbContext.Set<Aircraft>().Add(aircraft);
        //     await _dbContext.SaveChangesAsync();
        //     return CreatedAtAction(nameof(aircraft), new { RegNumber = aircraft.RegNumber }, aircraft);
        // }

        // [HttpPut]
        // [TenantedAuthorize("AircraftEdit")]
        // [Route("{tenantId}/[controller]/{id}")]
        // public async Task<IActionResult> Put(string tenantId, string id, Aircraft aircraft)
        // {
        //     var a = await _dbContext.Set<Aircraft>().Where(a => a.RegNumber == id).FirstOrDefaultAsync();
        //     if (a == null)
        //         return NotFound();
        //     if (aircraft.IsGrounded)
        //         a.Ground();
        //     await _dbContext.SaveChangesAsync();
        //     return Ok();
        // }
    }
}