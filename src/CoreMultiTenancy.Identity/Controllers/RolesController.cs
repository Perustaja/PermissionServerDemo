using AutoMapper;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Attributes;
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
        [TenantedAuthorize]
        public async Task<IActionResult> GetOrganizationRoles(Guid orgId)
        {
            var roles = await _orgManager.GetRolesOfOrgAsync(orgId);
            if (roles.Count > 0)
            {
                var mappedRoles = _mapper.Map<List<RoleGetDto>>(roles);
                return Ok(mappedRoles);
            }
            throw new Exception($"Found no roles, org:{orgId}");
        }

        [HttpDelete("organizations/{orgId}/users/{userId}/roles/{roleId}")]
        [TenantedAuthorize(PermissionEnum.UsersManageRoles)]
        public async Task<IActionResult> RemoveRoleFromUser(Guid orgId, Guid userId, Guid roleId)
        {
            // need some permission check that the user has access & permissions
            var errOpt = await _orgManager.RemoveRoleFromUserAsync(userId, orgId, roleId);
            if (errOpt.IsSome())
            {
                var e = errOpt.Unwrap();
                if (e.ErrorType == Results.Errors.ErrorType.DomainLogic)
                    return BadRequest(e.Description);
                else
                    return NotFound(e.Description);
            }
            else
                return NoContent();
        }
    }
}