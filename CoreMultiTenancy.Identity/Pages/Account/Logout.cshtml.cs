using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [Authorize]
    [ValidateAntiForgeryToken]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly IIdentityServerInteractionService _interactionSvc;
        private readonly SignInManager<User> _signInManager;
        private IEventService _eventSvc;

        public LogoutModel(ILogger<LoginModel> logger,
            IConfiguration config,
            IIdentityServerInteractionService interactionSvc,
            SignInManager<User> signInManager,
            IEventService eventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interactionSvc = interactionSvc ?? throw new ArgumentNullException(nameof(interactionSvc));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _eventSvc = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string LogoutId { get; set; }
            public bool ShowLogoutPrompt { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            Input.LogoutId = logoutId;
            Input.ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt;

            // If user is not logged in, redirect them to the login page
            if (User?.Identity.IsAuthenticated != true)
                return RedirectToAction("Login");

            // Check if context requires logout prompt, if not it's safe to sign out
            var context = await _interactionSvc.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt != true)
            {
                Input.ShowLogoutPrompt = false;
                return Page();
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var vm = await BuildLoggedOutViewModel(Input.LogoutId);

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

            return RedirectToPage("LoggedOut", vm);
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
    }
}