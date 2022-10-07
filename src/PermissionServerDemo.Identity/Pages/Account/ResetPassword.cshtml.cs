using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using PermissionServerDemo.Identity.Extensions;
using PermissionServerDemo.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace PermissionServerDemo.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordModel(UserManager<User> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        //PRG TempData for Login page on success
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
            if (String.IsNullOrEmpty(code) || String.IsNullOrEmpty(userId))
                return RedirectToPage("/error/notfound");
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
                        RedirectResultMessage = "Your password has successfully been reset";
                        return RedirectToPage("/account/login");
                    }
                    this.AddIdentityResultErrors(result);
                    return Page();
                }
                ModelState.AddModelError("", "The link provided was invalid or has expired. Please have a valid link sent to your email.");
            }
            return Page();
        }
    }
}