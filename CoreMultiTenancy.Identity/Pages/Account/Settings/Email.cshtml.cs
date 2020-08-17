using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

        public EmailModel(ILogger<PageModel> logger, UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [EmailAddress]
            [Display(Name = "Current Email")]
            public string CurrentEmail { get; set; }
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
                return Page();
            }
            _logger.LogError($"{nameof(EmailModel)}: User authenticated but lookup returned null User object.");
            return RedirectToPage("error");
        }
    }
}