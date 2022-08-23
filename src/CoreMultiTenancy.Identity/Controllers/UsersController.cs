using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Entities.Dtos;
using CoreMultiTenancy.Identity.Interfaces;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Identity.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
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

        [HttpGet]
        [Route("/organizations")]
        public async Task<IActionResult> Get(Guid userId)
        {
            var orgs = await _orgManager.GetUserOrganizationsByUserIdAsync(new Guid(User.GetSubjectId()));
            var orgDtos = orgs.Count > 0
                ? _mapper.Map<List<UserOrganizationGetDto>>(orgs)
                : new List<UserOrganizationGetDto>();
            return Ok(orgDtos);
        }
    }
}