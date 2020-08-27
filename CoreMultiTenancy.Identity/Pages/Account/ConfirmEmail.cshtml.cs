using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly ILogger<ConfirmEmailModel> _logger;
        private readonly UserManager<User> _userManager;

        public ConfirmEmailModel(ILogger<ConfirmEmailModel> logger, UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ViewData]
        public bool Success { get; set; }

        [ViewData]
        public string ResultMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(code))
                return RedirectToPage("/error/notfound");
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    Success = true;
                    ResultMessage = "Your email has successfully been confirmed. Thank you.";
                    return Page();
                }
            }
            Success = false;
            ResultMessage = "The link provided was invalid or has expired. Please have a valid link sent to your email.";
            return Page();
        }
    }
}