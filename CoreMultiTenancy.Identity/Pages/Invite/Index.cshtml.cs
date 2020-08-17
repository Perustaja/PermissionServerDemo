using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Pages.Invite
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrganizationInviteService _inviteService;
        public IndexModel(UserManager<User> userManager,
            IOrganizationInviteService inviteService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _inviteService = inviteService ?? throw new ArgumentNullException(nameof(inviteService));
        }

        [TempData]
        public bool Success { get; set; }
        [TempData]
        public string ResultMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> OnGetAsync(string inviteCode)
        {
            // Verify user is logged in and attempt to use invite code
            if (User?.Identity.IsAuthenticated == true)
            {
                var userId = User.GetSubjectId();
                var user = await _userManager.FindByIdAsync(userId);
                var invResult = await _inviteService.UsePermInvitationLink(user, inviteCode);
                // Display view with appropriate message and status
                Success = invResult.Success;
                ResultMessage = invResult.Success ? invResult.SuccessMessage : invResult.ErrorMessage;
                return Page();
            }
            return RedirectToPage("Login", new { ReturnUrl = $"/Invite/{inviteCode}" });
        }
    }
}