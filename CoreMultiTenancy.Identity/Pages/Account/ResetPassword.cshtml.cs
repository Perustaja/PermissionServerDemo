using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public class ResetPasswordModel : PageModel
    {
        private readonly ILogger<ResetPasswordModel> _logger;
        private readonly UserManager<User> _userManager;

        public ResetPasswordModel(ILogger<ResetPasswordModel> logger,
            UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmNewPassword { get; set; }

            public string UserId { get; set; }

            public string Code { get; set; }
        }
        public IActionResult OnGetAsync(string code, string Id)
        {
            if (String.IsNullOrWhiteSpace(code) || Id == null)
                return RedirectToPage("error");
            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
                UserId = Id
            };
            return Page();
        }

        // public async Task<IActionResult> OnPostAsync()
        // {
        //     if (ModelState.IsValid)
        //     {
        //         // attempt to retrieve user object
        //         var user = await _userManager.FindByIdAsync(Input.UserId);
        //         if (user != null)
        //         {
        //             // attempt to reset password
        //             var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.ConfirmNewPassword);
        //         }

        //     }
        //     // redirect with appropriate error message
        // }
    }
}