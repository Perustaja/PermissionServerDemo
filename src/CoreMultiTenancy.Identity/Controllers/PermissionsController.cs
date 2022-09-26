using AutoMapper;
using CoreMultiTenancy.Identity.Entities.Dtos;
using CoreMultiTenancy.Identity.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace CoreMultiTenancy.Identity.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [Authorize(LocalApi.PolicyName)]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permSvc;
        private readonly IOrganizationManager _orgManager;
        private readonly IMapper _mapper;

        public PermissionsController(IPermissionService permSvc,
            IOrganizationManager orgManager,
            IMapper mapper)
        {
            _permSvc = permSvc ?? throw new ArgumentNullException(nameof(permSvc));
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("permissionCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var perms = await _permSvc.GetAllPermissionCategoriesAsync();
            var mappedPerms = _mapper.Map<List<PermissionCategoryGetDto>>(perms);
            return Ok(mappedPerms);
        }

        [HttpGet("users/{userId}/organizations/{orgId}/permissions")]
        public async Task<IActionResult> GetPermissionsWithinTenant(Guid userId, Guid orgId)
        {
            var tokenId = new Guid(User.GetSubjectId());
            if (userId == tokenId)
            {
                if (await _orgManager.UserHasAccessAsync(userId, orgId))
                {
                    var perms = await _permSvc.GetUsersPermissionsAsync(userId, orgId);
                    if (perms.Count > 0)
                    {
                        return Ok(perms.Select(p => p.ToString()));
                    }

                    throw new Exception($"User: {userId}, Org: {orgId} User has access but no permissions.");
                }

                return Forbid($"User {userId} does not have access to organization {orgId}");
            }

            return BadRequest($"userId in the URI must match the userId within the access token. Token id: {tokenId}");
        }
    }
}