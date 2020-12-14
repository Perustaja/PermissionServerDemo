using System;
using System.Text;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class ChangeEmailConfirmationModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ChangeEmailConfirmationModel(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [ViewData]
        public bool Success { get; set; }
        [ViewData]
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