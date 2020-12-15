using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class ResendConfirmationEmailModel : PageModel
    {
        private readonly IAccountEmailService _acctEmailService;

        public ResendConfirmationEmailModel(IAccountEmailService acctEmailService)
        {
            _acctEmailService = acctEmailService ?? throw new ArgumentNullException(nameof(acctEmailService));
        }

        [ViewData]
        public string SuccessMessage { get; set; }

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

        /// <summary>
        /// Attempts to send email confirmation, updates page with results.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _acctEmailService.SendConfToUnauthUserAsync(Input.Email);
                // Update viewdata, redisplay form
                if (result.Approved)
                    SuccessMessage = result.Message;
                else
                    ModelState.AddModelError("", result.Message);

            }
            return Page();
        }
    }
}