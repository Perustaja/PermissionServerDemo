using AutoMapper;
using PermissionServerDemo.Api.Data;
using PermissionServerDemo.Api.Entities;
using PermissionServerDemo.Api.Entities.Dtos;
using PermissionServerDemo.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PermissionServerDemo.Core.Attributes;

namespace PermissionServerDemo.Api.Controllers
{
    [Authorize]
    [ApiController]
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
        [RemoteAuthorize]
        [Route("{tenantId}/aircraft")]
        public async Task<IActionResult> Get(Guid tenantId)
        {
            var ac = await _dbContext.Set<Aircraft>()
                .ToListAsync();
            var mappedAc = _mapper.Map<List<AircraftGetDto>>(ac);
            return Ok(mappedAc);
        }

        [HttpPost]
        [Route("{tenantId}/aircraft")]
        [RemoteAuthorize(PermissionEnum.AircraftCreate)]
        public async Task<IActionResult> Post(Guid tenantId, [FromBody] AircraftCreateDto dto)
        {
            if (await _dbContext.Set<Aircraft>().AnyAsync(a => a.RegNumber == dto.RegNumber))
                return Conflict($"Aircraft already exists with registration number: {dto.RegNumber}");

            var ac = new Aircraft(dto.RegNumber, tenantId, dto.ThumbnailUri, dto.Model);
            _dbContext.Set<Aircraft>().Add(ac);
            await _dbContext.Commit();
            return Created("api/v{version:apiVersion}/organizations/aircraft/" + ac.RegNumber, ac);
        }
    }
}