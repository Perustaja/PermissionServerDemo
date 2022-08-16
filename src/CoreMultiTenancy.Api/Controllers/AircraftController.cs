using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Api.Authorization;
using CoreMultiTenancy.Api.Data;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Core.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AircraftController : ControllerBase
    {
        private readonly TenantedDbContext _dbContext;

        public AircraftController(TenantedDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        [TenantedAuthorize]
        [Route("{tenantId}/[controller]")]
        public async Task<IActionResult> Get(string tenantId)
            => Ok(await _dbContext.Set<Aircraft>().ToListAsync());

        [HttpPost]
        [TenantedAuthorize(PermissionEnum.AircraftCreate)]
        [Route("{tenantId}/[controller]")]
        public async Task<IActionResult> Post(string tenantId, Aircraft aircraft)
        {
            if (await _dbContext.Set<Aircraft>().AnyAsync(a => a.RegNumber == aircraft.RegNumber))
                return Conflict($"Aircraft already exists with registration number: {aircraft.RegNumber}");

            _dbContext.Set<Aircraft>().Add(aircraft);
            await _dbContext.Commit();
            return CreatedAtAction(nameof(aircraft), new { RegNumber = aircraft.RegNumber }, aircraft);
        }

        [HttpPut]
        [TenantedAuthorize(PermissionEnum.AircraftEdit)]
        [Route("{tenantId}/[controller]/{id}")]
        public async Task<IActionResult> Put(string tenantId, string id, Aircraft aircraft)
        {
            var a = await _dbContext.Set<Aircraft>().Where(a => a.RegNumber == id).FirstOrDefaultAsync();
            if (a == null)
                return NotFound();
            if (aircraft.IsGrounded)
                a.Ground();
            await _dbContext.Commit();
            return Ok();
        }
    }
}