using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
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
    [SecurityHeaders]
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
        public string SuccessMessage { get; set; }

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
            _logger.LogEmptyAuthenticatedUser(user);
            return RedirectToPage("error");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    if (user.Email != Input.NewEmail)
                    {
                        var token = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                        var callbackUrl = Url.ConfirmEmailPageLink(userId, token, Request.Scheme);
                        await _emailSender.SendEmailChangeEmail(Input.NewEmail, callbackUrl);
                        SuccessMessage = $"An email containing a link to confirm your email change has been sent to {Input.NewEmail}.";
                        SetPrepopulatedFormData(user);
                        return Page();
                    }
                    ModelState.AddModelError(String.Empty, "The entered email is the same as your existing email.");
                    SetPrepopulatedFormData(user);
                    return Page();
                }
                SetPrepopulatedFormData(user);
                return Page();
            }
            _logger.LogEmptyAuthenticatedUser(user);
            return RedirectToPage("error");
        }

        private void SetPrepopulatedFormData(User user)
        {
            CurrentEmail = user.Email;
            EmailConfirmed = user.EmailConfirmed;
        }
    }
}