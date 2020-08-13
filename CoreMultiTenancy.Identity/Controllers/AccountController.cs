using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.ViewModels.Account;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _config;
        private readonly IIdentityServerInteractionService _interactionSvc;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEventService _eventSvc;
        public AccountController(ILogger<AccountController> logger,
        IConfiguration config,
        IIdentityServerInteractionService interactionSvc,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEventService eventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _interactionSvc = interactionSvc ?? throw new ArgumentNullException(nameof(interactionSvc));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _eventSvc = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Settings()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interactionSvc.GetAuthorizationContextAsync(returnUrl);
            // Redirect as necessary if user is already logged in
            if (User?.Identity.IsAuthenticated == true)
                return RedirectUponLogin(context, returnUrl);

            if (context?.IdP != null)
            {
                _logger.LogWarning("External login service requested, but not implemented.");
                return NotFound();
            }

            var vm = new LoginViewModel()
            {
                Email = context?.LoginHint,
                ReturnUrl = returnUrl,
            };
            ViewData["ReturnUrl"] = returnUrl;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            // Get context of request
            var context = await _interactionSvc.GetAuthorizationContextAsync(vm.ReturnUrl);

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(vm.Email);
                    await _eventSvc.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));
                    // Login successful and logged, now redirect user
                    return RedirectUponLogin(context, vm.ReturnUrl);
                }
                ModelState.AddModelError("", "Invalid email or password.");
            }
            // Return view with errors
            ViewData["ReturnUrl"] = vm.ReturnUrl;
            return View(vm);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            var newUser = new User(vm.FirstName, vm.LastName, vm.Email);
            var res = await _userManager.CreateAsync(newUser, vm.Password);
            if (res.Errors.Count() > 0)
            {
                AddErrors(res);
                return View(vm);
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = new LogoutViewModel() { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            // If user is not logged in, redirect them to the login page
            if (User?.Identity.IsAuthenticated != true)
                return RedirectToAction("Login");

            // Check if context requires logout prompt, if not it's safe to sign out
            var context = await _interactionSvc.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt != true)
            {
                vm.ShowLogoutPrompt = false;
                return View(vm);
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel im)
        {
            var vm = await BuildLoggedOutViewModel(im.LogoutId);

            // Show login page if user is not logged in currently
            if (User?.Identity.IsAuthenticated != true)
                return RedirectToAction("Login");

            // delete local authentication cookie
            await _signInManager.SignOutAsync();

            // raise the logout event
            await _eventSvc.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }
        private async Task<LoggedOutViewModel> BuildLoggedOutViewModel(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var context = await _interactionSvc.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = context?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(context?.ClientName) ? context?.ClientId : context?.ClientName,
                SignOutIframeUrl = context?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            // Handle user sign out if using an external provider
            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interactionSvc.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }
            return vm;
        }
        private void AddErrors(IdentityResult res)
        {
            foreach (var err in res.Errors)
                ModelState.AddModelError(String.Empty, err.Description);
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
                return Redirect(returnUrl);
            }
            // Return to home if ReturnUrl is null or invalid
            else
            {
                return Redirect("~/");
            }
        }
    }
}