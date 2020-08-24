using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account.Settings
{
    [Authorize]
    [ValidateAntiForgeryToken]
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

        [TempData]
        public bool Success { get; set; }
        [TempData]
        public string ResultMessage { get; set; }

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
                    Success = result.Succeeded;
                    if (result.Succeeded)
                    {
                        ResultMessage = $"Your password has successfully been changed.";
                        await _signInManager.RefreshSignInAsync(user);
                        return Page();
                    }
                    ResultMessage = "The current password entered is incorrect.";
                    return Page();
                }
                _logger.LogError($"User authenticated but lookup returned null User object.");
                return RedirectToPage("error");
            }
            return Page();
        }
    }
}