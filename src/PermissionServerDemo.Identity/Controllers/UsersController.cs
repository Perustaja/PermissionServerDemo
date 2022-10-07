using AutoMapper;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Entities.Dtos;
using PermissionServerDemo.Identity.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace PermissionServerDemo.Identity.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(LocalApi.PolicyName)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrganizationManager _orgManager;
        private readonly IMapper _mapper;

        public UsersController(UserManager<User> userManager,
            IOrganizationManager orgManager,
            IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userId}/organizations")]
        public async Task<IActionResult> GetOrganizations(Guid userId)
        {
            var tokenId = new Guid(User.GetSubjectId());
            if (userId == tokenId)
            {
                var orgs = await _orgManager.GetUserOrganizationsByUserIdAsync(userId);
                var orgDtos = orgs.Count > 0
                    ? _mapper.Map<List<UserOrganizationGetDto>>(orgs)
                    : new List<UserOrganizationGetDto>();
                return Ok(orgDtos);
            }

            return BadRequest($"userId in the URI must match the userId within the access token. Token id: {tokenId}");
        }
    }
}