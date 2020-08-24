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
    public class EmailModel : PageModel
    {
        private readonly ILogger<PageModel> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(ILogger<PageModel> logger,
            UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        [ViewData]
        public bool Success { get; set; }
        [ViewData]
        public string ResultMessage { get; set; }

        [ViewData]
        [EmailAddress]
        [Display(Name = "Current Email")]
        public string CurrentEmail { get; set; }

        [ViewData]
        public bool EmailConfirmed { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New Email")]
            public string NewEmail { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                SetPrepopulatedFormData(user);
                return Page();
            }
            _logger.LogError($"User authenticated but lookup returned null User object.");
            return RedirectToPage("error");
        }

        public async Task<IActionResult> OnGetResendConfirmationAsync()
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _emailSender.SendAccountConfirmationEmail(user.Email, token);
                    Success = true;
                    ResultMessage = "A confirmation has been sent to your email. You may need to check your spam folder.";
                    SetPrepopulatedFormData(user);
                    return Page();
                }
                Success = false;
                ResultMessage = "Your email has already been confirmed.";
                SetPrepopulatedFormData(user);
                return Page();
            }
            _logger.LogError($"User authenticated but lookup returned null User object.");
            return RedirectToPage("error");
        }

        private void SetPrepopulatedFormData(User user)
        {
            CurrentEmail = user.Email;
            EmailConfirmed = user.EmailConfirmed;
        }
    }
}