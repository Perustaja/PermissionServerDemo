using System;
using System.ComponentModel.DataAnnotations;
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
    public class ResetPasswordModel : PageModel
    {
        private readonly ILogger<ResetPasswordModel> _logger;
        private readonly UserManager<User> _userManager;

        public ResetPasswordModel(ILogger<ResetPasswordModel> logger,
            UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        //PRG TempData for ResetPasswordConfirmation page
        [TempData]
        public bool RedirectSuccess { get; set; }
        [TempData]
        public string RedirectResultMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string UserId { get; set; }

            public string Code { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmNewPassword { get; set; }
        }
        public IActionResult OnGetAsync(string userId, string code)
        {
            if (String.IsNullOrWhiteSpace(code) || String.IsNullOrWhiteSpace(userId))
                return RedirectToPage("notfound");
            // set hidden model values
            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
                UserId = userId
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // attempt to retrieve user object
                var user = await _userManager.FindByIdAsync(Input.UserId);
                if (user != null)
                {
                    // attempt to reset password
                    var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.ConfirmNewPassword);
                    if (result.Succeeded)
                    {
                        RedirectSuccess = true;
                        RedirectResultMessage = "Your password has successfully been reset";
                        return RedirectToPage("/account/passwordreset");
                    }
                }
                // Redirect with error message
                RedirectSuccess = false;
                RedirectResultMessage = "The link provided was invalid or has expired. Please have a valid link sent to your email.";
                return RedirectToPage("/account/passwordreset");
            }
            return Page();
        }
    }
}