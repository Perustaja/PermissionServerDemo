using AutoMapper;
using PermissionServerDemo.Identity.Attributes;
using PermissionServerDemo.Identity.Entities.Dtos;
using PermissionServerDemo.Identity.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace PermissionServerDemo.Identity.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
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
        [TenantedAuthorize]
        public async Task<IActionResult> GetPermissionsWithinTenant(Guid userId, Guid orgId)
        {
            // modified for workaround for demo
            var tokenId = new Guid(User.GetSubjectId());
            if (userId == tokenId)
            {
                var perms = await _permSvc.GetUsersPermissionsAsync(userId, orgId);
                if (perms.Count > 0)
                {
                    var isShadow = await _orgManager.IsUserOwnerAsync(userId, orgId);
                    return Ok(new PermResult(perms.Select(p => p.ToString()).ToArray(), !isShadow));
                }

                throw new Exception($"User: {userId}, Org: {orgId} User has access but no permissions.");
            }

            return BadRequest($"userId in the URI must match the userId within the access token. Token id: {tokenId}");
        }
    }
}

public class PermResult {
    public PermResult(string[] perms, bool isTenantShadow)
    {
        Permissions = perms;
        IsTenantShadow = isTenantShadow;
    }
    public string[] Permissions { get; set; }
    public bool IsTenantShadow { get; set; }
}