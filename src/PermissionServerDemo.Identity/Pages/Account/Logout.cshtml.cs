using System;
using System.Threading.Tasks;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Options;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PermissionServerDemo.Identity.Pages.Account
{
    [Authorize]
    [ValidateAntiForgeryToken]
    [SecurityHeaders]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interactionSvc;
        private readonly SignInManager<User> _signInManager;
        private readonly OidcAccountOptions _oidcAccountOptions;
        private IEventService _eventSvc;

        public LogoutModel(ILogger<LogoutModel> logger,
            IConfiguration config,
            IIdentityServerInteractionService interactionSvc,
            SignInManager<User> signInManager,
            IOptions<OidcAccountOptions> optionsAccessor,
            IEventService eventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interactionSvc = interactionSvc ?? throw new ArgumentNullException(nameof(interactionSvc));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _oidcAccountOptions = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _eventSvc = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // Data for post-signout results. Breaks razor page conventions but necessary for security.
        public LoggedOutViewModel LogoutResult { get; set; }

        public class LoggedOutModel
        {
            public string LogoutId { get; set; }
            public string PostLogoutRedirectUri { get; set; }
            public string ClientName { get; set; }
            public string SignOutIframeUrl { get; set; }
            public bool AutomaticRedirectAfterSignOut { get; set; }
            public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
            public string ExternalAuthenticationScheme { get; set; }
        }
        public class InputModel
        {
            public string LogoutId { get; set; }
            public bool ShowLogoutPrompt { get; set; }
        }

        /// <summary>
        /// Logs the user out. Logout should be called via the end-session endpoint.
        /// </summary>
        /// <param name="logoutId">
        /// Internal id used to store state. If null, it is created by idsrv. ValidateAntiForgeryToken
        /// ensures that this value is not manipulated in the form, so it is safe to not validate (you can't validate it anyway).
        /// </param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            Input = new InputModel
            {
                LogoutId = logoutId,
                ShowLogoutPrompt = _oidcAccountOptions.ShowLogoutPrompt
            };

            // If somehow user is not logged in, show login page
            if (User?.Identity.IsAuthenticated != true)
            {
                return RedirectToPage("Login");
            }

            // Check if context requires logout prompt, if not it's safe to sign out
            var context = await _interactionSvc.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
                return await OnPostAsync();

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return Page();
        }

        /// <summary>
        /// Logs user out and returns page. Redirecting to a loggedOut page in a secure way without ugly
        /// query string params in the url while maintaining razor page conventions is tricky. This is a tradeoff.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            LogoutResult = await BuildLoggedOutViewModelAsync(Input.LogoutId);

            // Show login page if user is not logged in currently
            if (User?.Identity.IsAuthenticated != true)
                return RedirectToAction("Login");

            // delete local authentication cookie
            await _signInManager.SignOutAsync();

            // raise the logout event
            await _eventSvc.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            // check if we need to trigger sign-out at an upstream identity provider
            if (LogoutResult.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = LogoutResult.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, LogoutResult.ExternalAuthenticationScheme);
            }

            return Page(); // Page will update if user is logged out, and redirect if necessary.
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var context = await _interactionSvc.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = _oidcAccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = context?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(context?.ClientName) ? context?.ClientId : context?.ClientName,
                SignOutIframeUrl = context?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            // Handle user sign out if using an external provider
            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var provider = HttpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                    var handler = await provider.GetHandlerAsync(HttpContext, idp);
                    if (handler is IAuthenticationSignOutHandler)
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