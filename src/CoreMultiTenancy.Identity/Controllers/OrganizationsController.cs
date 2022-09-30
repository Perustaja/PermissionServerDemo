using AutoMapper;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Attributes;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Entities.Dtos;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Results.Errors;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace CoreMultiTenancy.Identity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(LocalApi.PolicyName)]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationManager _orgManager;
        private readonly IMapper _mapper;

        public OrganizationsController(UserManager<User> userManager,
            IOrganizationManager orgManager,
            IMapper mapper)
        {
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{orgId}/users")]
        [TenantedAuthorize]
        public async Task<IActionResult> GetOrganizationUsers(Guid orgId)
        {
            var userOrgs = await _orgManager.GetUsersOfOrgAsync(orgId);
            var users = userOrgs.Select(uo =>
                new UsersGetDto()
                {
                    FirstName = uo.User.FirstName,
                    LastName = uo.User.LastName,
                    UserOrganization = _mapper.Map<UserOrganizationGetDto>(uo),
                    Roles = _mapper.Map<List<RoleGetDto>>(uo.User.UserOrganizationRoles.Select(uor => uor.Role).ToList())
                }
            );

            return Ok(users);
        }

        [HttpDelete("{orgId}/users/{userId}")]
        [TenantedAuthorize(PermissionEnum.UsersManageAccess)]
        public async Task<IActionResult> RevokeTenantAccessForUser(Guid orgId, Guid userId)
        {
            var errOpt = await _orgManager.RevokeAccessAsync(userId, orgId);
            if (errOpt.IsSome())
            {
                var err = errOpt.Unwrap();
                if (err.ErrorType == ErrorType.NotFound)
                    return NotFound(err.Description);
                else
                    return BadRequest(err.Description);
            }
            else
                return NoContent();
        }
    }
}