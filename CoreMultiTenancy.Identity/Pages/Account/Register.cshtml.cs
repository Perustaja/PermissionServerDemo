using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
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
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;
        private readonly UserManager<User> _userManager;
        public RegisterModel(ILogger<RegisterModel> logger,
        UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(25, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            var newUser = new User(Input.FirstName, Input.LastName, Input.Email);
            var res = await _userManager.CreateAsync(newUser, Input.Password);
            if (res.Errors.Count() > 0)
            {
                this.AddIdentityResultErrors(res);
                return Page();
            }

            return RedirectToPage("Login");
        }
    }
}