using AutoMapper;
using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Entities.Dtos;
using PermissionServerDemo.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;
using PermissionServerDemo.Core.Attributes;

namespace PermissionServerDemo.Identity.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
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
        [LocalAuthorize]
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

        [HttpPost("organizations/{orgId}/roles")]
        [LocalAuthorize(PermissionEnum.RolesCreate)]
        public async Task<IActionResult> CreateOrganizationRole(Guid orgId, [FromBody] RoleCreateDto dto)
        {
            var perms = new List<PermissionEnum>();
            foreach (var p in dto.Permissions)
            {
                if (Enum.TryParse<PermissionEnum>(p, out var perm))
                    perms.Add(perm);
                else
                    return BadRequest($"Unable to parse ${p} to a permission.");
            }

            var r = new Role(dto.Name, dto.Description);
            await _orgManager.AddRoleToOrgAsync(orgId, r, perms);
            return Created($"organizations/{orgId}/roles", null);
        }

        [HttpDelete("organizations/{orgId}/users/{userId}/roles/{roleId}")]
        [LocalAuthorize(PermissionEnum.UsersManageRoles)]
        public async Task<IActionResult> RemoveRoleFromUser(Guid orgId, Guid userId, Guid roleId)
        {
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