using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using PermissionServerDemo.Identity.Extensions;
using PermissionServerDemo.Identity.Interfaces;
using PermissionServerDemo.Identity.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PermissionServerDemo.Identity.Pages.Account.Settings
{
    [Authorize]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class PasswordModel : PageModel
    {
        private readonly ILogger<PageModel> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public PasswordModel(ILogger<PageModel> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [ViewData]
        public string SuccessMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; }
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmNewPassword { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.ConfirmNewPassword);
                    if (result.Succeeded)
                    {
                        SuccessMessage = $"Your password has successfully been changed.";
                        await _signInManager.RefreshSignInAsync(user);
                        return Page();
                    }
                    ModelState.AddModelError(String.Empty, "The current password entered is incorrect.");
                    return Page();
                }
                _logger.LogEmptyAuthenticatedUser(user);
                return RedirectToPage("error");
            }
            return Page();
        }
    }
}