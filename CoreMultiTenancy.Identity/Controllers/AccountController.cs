using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.ViewModels.Account;
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
        public AccountController(ILogger<AccountController> logger,
        IConfiguration config,
        IIdentityServerInteractionService interactionSvc,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
        {
            _logger = logger;
            _config = config;
            _interactionSvc = interactionSvc;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interactionSvc.GetAuthorizationContextAsync(returnUrl);
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
                    // Redirect to Portal if tenant selection unset
                    if (user.SelectedOrg == Guid.Empty)
                    {
                        return RedirectToAction("Index", "Portal", new { ReturnUrl = vm.ReturnUrl });
                    }
                    // Else if ReturnUrl is valid redirect
                    else if (context != null)
                    {
                        if (context.IsNativeClient())
                        {
                            return this.LoadingPage("Redirect", vm.ReturnUrl);
                        }
                        return Redirect(vm.ReturnUrl);
                    }
                    // Else if local, redirect
                    else if (Url.IsLocalUrl(vm.ReturnUrl))
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    // Return to home if ReturnUrl is null or invalid
                    else
                    {
                        return Redirect("~/");
                    }
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
        private void AddErrors(IdentityResult res)
        {
            foreach (var err in res.Errors)
                ModelState.AddModelError(String.Empty, err.Description);
        }
    }
}