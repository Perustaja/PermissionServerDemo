using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class ForgotPasswordModel : PageModel
    {
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(ILogger<ForgotPasswordModel> logger,
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

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; }
        }
        public IActionResult OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/account/resetpassword",
                            pageHandler: null,
                            values: new { userId = user.Id, code = token },
                            protocol: Request.Scheme);
                        await _emailSender.SendPasswordResetEmail(user.Email, callbackUrl);
                        Success = true;
                        ResultMessage = "If an account exists with this email, a password reset link has been emailed which will expire in 24 hours.";
                        return Page();
                    }
                    Success = false;
                    ResultMessage = "The account associated with this email address has not been confirmed. Please confirm your account before resetting your password.";
                    return Page();
                }
                Success = true;
                ResultMessage = "If an account exists with this email, a password reset link has been emailed which will expire in 24 hours.";
            }
            return Page();
        }
    }
}