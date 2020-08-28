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

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class ResendConfirmationEmailModel : PageModel
    {
        private readonly ILogger<PageModel> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendConfirmationEmailModel(ILogger<ResendConfirmationEmailModel> logger,
            UserManager<User> userManager,
            IEmailSender emailSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }

        //PRG TempData for Login page on success
        [TempData]
        public string RedirectResultMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // If user is logged in, use specific handler
            if (User?.Identity.IsAuthenticated == true)
                return await ResendToAuthenticatedUserAsync();
            // else return normal form
            return Page();
        }

        /// <summary>
        /// Should be used for normal form submission when a user is not authenticated.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    // Do not notify an unauthenticated user whether or not an email is verified
                    // Return the same results but, if it is confirmed, just silently do nothing.
                    if (!user.EmailConfirmed)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        await _emailSender.SendAccountConfirmationEmail(user.Email, token);
                    }
                    RedirectResultMessage = $"A confirmation link has been sent to {user.Email} if this account is not verified already. You may need to check your spam folder.";
                    return RedirectToPage("/account/login");
                }
                // The user can easily figure out if an email is registered by attempting to make an
                // account under it, so exposing this detail here is not a security concern.
                ModelState.AddModelError("", "No account associated with the entered email exists.");
            }
            return Page();
        }

        /// <summary>
        /// Should be used if the user is already authenticated. This allows a logged-in user to
        /// not have to fill out the form manually, and also prevents them from manually entering in
        /// email addresses that do not belong to them.
        /// </summary>
        private async Task<IActionResult> ResendToAuthenticatedUserAsync()
        {
            // Make 100% sure user is still authenticated
            if (User?.Identity.IsAuthenticated == true)
            {
                var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // User should not encounter redirect unless they manually make a request to this page
                    if (user.EmailConfirmed)
                    {  
                        RedirectResultMessage = "Your account's email is already confirmed.";
                        return RedirectToPage("/account/login");
                    }
                    // Else, resend confirmation
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _emailSender.SendAccountConfirmationEmail(user.Email, token);
                    RedirectResultMessage = $"A confirmation link has been sent to {user.Email}. You may need to check your spam folder.";
                    return RedirectToPage("/account/login");
                }
                _logger.LogWarning("User authenticated but UserManager returned null object.");
                return RedirectToPage("/error");
            }
            // else return normal form if they logged out somehow
            return Page();
        }
    }
}