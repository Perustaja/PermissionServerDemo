using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
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
        private readonly IAccountEmailService _acctEmailService;

        public EmailModel(ILogger<PageModel> logger,
            UserManager<User> userManager,
            IAccountEmailService acctEmailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _acctEmailService = acctEmailService ?? throw new ArgumentNullException(nameof(acctEmailService));
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

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Manually check email field so multiple forms can be present on page
                if (!String.IsNullOrEmpty(Input.NewEmail))
                {
                    var res = await _acctEmailService.SendEmailChangeEmail(user.Email, Input.NewEmail);
                    if (res.Approved)
                        SuccessMessage = res.Message;
                    else
                        ModelState.AddModelError("", res.Message);
                    SetPrepopulatedFormData(user);
                    return Page();
                }
                ModelState.AddModelError("", "The NewEmail field is required.");
                SetPrepopulatedFormData(user);
                return Page();
            }
            _logger.LogEmptyAuthenticatedUser(user);
            return RedirectToPage("error");
        }

        public async Task<IActionResult> OnPostSendConfAsync()
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var res = await _acctEmailService.SendConfToAuthUserAsync(user);
                if (res.Approved)
                    SuccessMessage = res.Message;
                else
                    ModelState.AddModelError("", res.Message);
                SetPrepopulatedFormData(user);
                return Page();
            }
            _logger.LogEmptyAuthenticatedUser(user);
            return RedirectToPage("error");
        }

        [NonHandler]
        private void SetPrepopulatedFormData(User user)
        {
            CurrentEmail = user.Email;
            EmailConfirmed = user.EmailConfirmed;
        }
    }
}