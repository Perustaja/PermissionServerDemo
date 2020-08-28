using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
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
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;
        private readonly UserManager<User> _userManager;

        public ProfileModel(ILogger<ProfileModel> logger, UserManager<User> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [ViewData]
        public bool Success { get; set; }
        [ViewData]
        public string ResultMessage { get; set; }

        [ViewData]
        public string CurrentFirstName { get; set; }
        [ViewData]
        public string CurrentLastName { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(25, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }
        public async Task<IActionResult> OnGet()
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
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    user.UpdateName(Input.FirstName, Input.LastName);
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        ResultMessage = "Profile settings updated. Please allow up to an hour for changes to be reflected.";
                    else
                    {
                        ResultMessage = "There was an error updating your profile settings.";
                        _logger.LogError($"Unable to update User profile information. FirstName: {Input?.FirstName}, LastName: {Input?.LastName}.");
                    }
                    Success = result.Succeeded;
                    SetPrepopulatedFormData(user);
                    return Page();
                }
                _logger.LogEmptyAuthenticatedUser(user);
                return RedirectToPage("error");
            }
            SetPrepopulatedFormData(user);
            return Page();
        }
        private void SetPrepopulatedFormData(User user)
        {
            CurrentFirstName = user.FirstName;
            CurrentLastName = user.LastName;
        }
    }
}