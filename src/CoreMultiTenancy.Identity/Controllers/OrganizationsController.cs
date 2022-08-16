using System;
using System.IO;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Entities.Dtos;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Identity.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OrganizationsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrganizationManager _orgManager;

        public OrganizationsController(UserManager<User> userManager,
            IOrganizationManager orgManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
        }
    }
}