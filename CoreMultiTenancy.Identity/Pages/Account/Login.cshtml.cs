using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Models;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly IIdentityServerInteractionService _interactionSvc;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private IEventService _eventSvc;
        public LoginModel(ILogger<LoginModel> logger,
            IConfiguration config,
            IIdentityServerInteractionService interactionSvc,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEventService eventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interactionSvc = interactionSvc ?? throw new ArgumentNullException(nameof(interactionSvc));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _eventSvc = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var context = await _interactionSvc.GetAuthorizationContextAsync(returnUrl);
            // Redirect as necessary if user is already logged in
            if (User?.Identity.IsAuthenticated == true)
                return RedirectUponLogin(context, returnUrl);

            if (context?.IdP != null)
            {
                _logger.LogWarning("External login service requested, but not implemented.");
                return RedirectToPage("error");
            }

            var model = new InputModel()
            {
                Email = context?.LoginHint,
            };
            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            // Get context of request
            var context = await _interactionSvc.GetAuthorizationContextAsync(ReturnUrl);

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {

                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    await _eventSvc.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));
                    // Login successful and logged, now redirect user
                    return RedirectUponLogin(context, ReturnUrl);
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(String.Empty, $"Your account must be confirmed before logging in. Need a new confirmation link? <a href='/account/resendconfirmationemail'>Click here</a>");
                    return Page();
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(String.Empty, $"Your account is locked out. Please wait 10 minutes before attempting to sign in again.");
                    return Page();
                }
                ModelState.AddModelError(String.Empty, "Invalid email or password.");
            }
            // Return view with errors
            return Page();
        }
        /// <summary>
        /// Redirects the logged in user back to its native client, redirectUrl, or the Idp home page.
        /// </summary>
        private IActionResult RedirectUponLogin(AuthorizationRequest context, string returnUrl)
        {
            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    return this.LoadingPage("Redirect", returnUrl);
                }
                return Redirect(returnUrl);
            }
            // Else if local, redirect
            else if (Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            // Return to home if ReturnUrl is null or invalid
            else
            {
                return RedirectToPage("/home/index");
            }
        }
    }
}