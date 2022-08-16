using System;
using CoreMultiTenancy.Identity.Entities;
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
        private readonly IPermissionService _permSvc;

        public UsersController(UserManager<User> userManager,
            IPermissionService permSvc)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _permSvc = permSvc ?? throw new ArgumentNullException(nameof(permSvc));
        }

        [HttpGet]
        [Route("/{orgId}/myPermissions")]
        public IActionResult GetMyPermissions(string orgId)
        {
            var perms = _permSvc.GetUsersPermissionsAsync(new Guid(User.GetSubjectId()), new Guid(orgId));
            return Ok(perms);
        }
    }
}