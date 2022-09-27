using AutoMapper;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Entities.Dtos;
using CoreMultiTenancy.Identity.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace CoreMultiTenancy.Identity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/")]
    [Authorize(LocalApi.PolicyName)]
    public class RolesController : ControllerBase
    {
        private readonly IOrganizationManager _orgManager;
        private readonly IMapper _mapper;

        public RolesController(UserManager<User> userManager,
            IOrganizationManager orgManager,
            IMapper mapper)
        {
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("organizations/{orgId}/roles")]
        public async Task<IActionResult> GetOrganizationRoles(Guid orgId)
        {
            var userId = new Guid(User.GetSubjectId());
            if (await _orgManager.UserHasAccessAsync(userId, orgId))
            {
                var roles = await _orgManager.GetRolesOfOrgAsync(orgId);
                if (roles.Count > 0)
                {
                    var mappedRoles = _mapper.Map<List<RoleGetDto>>(roles);
                    return Ok(mappedRoles);
                }
                throw new Exception($"Found no roles, org:{orgId}");
            }

            return BadRequest($"Organization {orgId} doesn't exist or user {userId} does not have access.");
        }
    }
}