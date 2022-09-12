using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Duende.IdentityServer.Extensions;

namespace CoreMultiTenancy.Identity.Pages.Invite
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IOrganizationManager _orgManager;
        public IndexModel(UserManager<User> userManager,
            IOrganizationManager orgManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
        }

        [TempData]
        public bool Success { get; set; }
        [TempData]
        public string ResultMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string inviteCode)
        {
            if (String.IsNullOrWhiteSpace(inviteCode))
            {
                Success = false;
                ResultMessage = "No invitation code was found.";
                return Page();
            }
            // Verify user is logged in and attempt to use invite code
            if (User?.Identity.IsAuthenticated == true)
            {
                var userId = User.GetSubjectId();
                var user = await _userManager.FindByIdAsync(userId);
                var invResult = await _orgManager.UsePermanentInvitationAsync(user, inviteCode);
                // Display view with appropriate message and status
                Success = invResult.Success;
                ResultMessage = invResult.Success ? invResult.SuccessMessage : invResult.ErrorMessage;
                return Page();
            }
            return RedirectToPage("Login", new { ReturnUrl = $"/Invite/{inviteCode}" });
        }
    }
}