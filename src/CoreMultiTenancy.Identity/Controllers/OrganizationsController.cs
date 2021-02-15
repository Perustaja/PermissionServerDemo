using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Create a new tenant for testing
            Organization o = new Organization("test", false);
            var orgOpt = await _orgManager.AddAsync(o);

            return orgOpt.MapOrElse<IActionResult>
            (
                () => StatusCode(StatusCodes.Status500InternalServerError),
                o => Created("test", o)
            );
        }
    }
}