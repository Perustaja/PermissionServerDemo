using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreMultiTenancy.Api.Authorization;
using CoreMultiTenancy.Api.Data;
using CoreMultiTenancy.Api.Entities;
using CoreMultiTenancy.Api.Entities.Dtos;
using CoreMultiTenancy.Core.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations")]
    public class AircraftController : ControllerBase
    {
        private readonly TenantedDbContext _dbContext;
        private readonly IMapper _mapper;

        public AircraftController(TenantedDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [TenantedAuthorize]
        [Route("{tenantId}/[controller]")]
        public async Task<IActionResult> Get(Guid tenantId)
        {
            var ac = await _dbContext.Set<Aircraft>()
                .Where(a => a.TenantId == tenantId)
                .ToListAsync();
            var mappedAc = _mapper.Map<AircraftGetDto>(ac);
            return Ok(mappedAc);
        }

        [HttpPost]
        [TenantedAuthorize(PermissionEnum.AircraftCreate)]
        [Route("{tenantId}/[controller]")]
        public async Task<IActionResult> Post(Guid tenantId, Aircraft aircraft)
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
        public async Task<IActionResult> Put(Guid tenantId, string id, Aircraft aircraft)
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