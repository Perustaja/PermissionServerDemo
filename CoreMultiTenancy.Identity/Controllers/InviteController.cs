using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    public class InviteController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrganizationInviteService _inviteService;
        public InviteController(UserManager<User> userManager,
            IOrganizationInviteService inviteService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _inviteService = inviteService ?? throw new ArgumentNullException(nameof(inviteService));
        }
        // GET Invite/{inviteCode}
        [HttpGet]
        [Route("Invite/{inviteCode}")]
        public async Task<IActionResult> Index(string inviteCode)
        {
            TempData["Success"] = false;
            // Verify user is logged in and attempt to use invite code
            if (User?.Identity.IsAuthenticated == true)
            {
                var userId = User.GetSubjectId();
                var user = await _userManager.FindByIdAsync(userId);
                var invResult = await _inviteService.UsePermInvitationLink(user, inviteCode);
                // Display view with appropriate message and status
                TempData["Success"] = invResult.Success;
                TempData["Message"] = invResult.Success ? invResult.SuccessMessage : invResult.ErrorMessage;
                return View();
            }
            return RedirectToAction("Login", "Account", new { ReturnUrl = $"/Invite/{inviteCode}" });
        }
    }
}