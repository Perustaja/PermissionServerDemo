using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using PermissionServerDemo.Identity.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PermissionServerDemo.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IAccountEmailService _acctEmailService;

        public ForgotPasswordModel(IAccountEmailService acctEmailService)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _acctEmailService.SendPassResetEmail(Input.Email);
                SuccessMessage = "If a verified account exists with this email, a password reset link has been sent to it.";
            }
            return Page();
        }
    }
}