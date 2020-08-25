using System;
using System.Text;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class ChangeEmailConfirmationModel : PageModel
    {
        private readonly ILogger<ChangeEmailConfirmationModel> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ChangeEmailConfirmationModel(ILogger<ChangeEmailConfirmationModel> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [TempData]
        public bool Success { get; set; }
        [TempData]
        public string ResultMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
                return RedirectToPage("error");

            // Retrieve associated User and change their email
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            // Note: ChangeEmailAsync also sets EmailConfirmed to true
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            Success = result.Succeeded;
            if (result.Succeeded)
            {
                // Currently, username is the same as email so we need to change both before success
                var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
                if (setUserNameResult.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    ResultMessage = "Your email has successfully been changed.";
                    return Page();
                }
            }
            ResultMessage = "The link you clicked was either expired or invalid. Please have a valid email change link sent to your email.";
            return Page();
        }
    }
}