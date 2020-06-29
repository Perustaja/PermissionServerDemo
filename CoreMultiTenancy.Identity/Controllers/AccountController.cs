using System;
using System.Linq;
using System.Threading.Tasks;
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var context = await _interactionSvc.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
                throw new NotImplementedException("External login not implemented.");

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
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (await _userManager.CheckPasswordAsync(user, vm.Password))
                {
                    var tknLifetime = _config.GetValue("TokenLifetimeMinutes", 60);
                    var props = new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(tknLifetime),
                        AllowRefresh = true,
                        RedirectUri = vm.ReturnUrl,
                    };
                    if (vm.RememberMe)
                    {
                        props.ExpiresUtc = DateTime.UtcNow.AddYears(1);
                        props.IsPersistent = true;
                    }

                    await _signInManager.SignInAsync(user, props);
                    if (_interactionSvc.IsValidReturnUrl(vm.ReturnUrl))
                    {
                        return Redirect(vm.ReturnUrl);
                    }
                    // Redirect to client home
                    return Redirect("~/");
                }
                ModelState.AddModelError("", "Invalid email or password.");
            }
            // Return view with errors
            ViewData["ReturnUrl"] = vm.ReturnUrl;
            return View(vm);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [AllowAnonymous]
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