using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.ViewModels.Account;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IIdentityServerInteractionService _interactionSvc;
        private readonly UserManager<User> _userManager;
        public AccountController(ILogger<AccountController> logger,
        IIdentityServerInteractionService interactionSvc,
        UserManager<User> userManager)
        {
            _logger = logger;
            _interactionSvc = interactionSvc;
            _userManager = userManager;
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
            
            return RedirectToAction("Index", "Portal");
        }
        private void AddErrors(IdentityResult res)
        {
            foreach (var err in res.Errors)
                ModelState.AddModelError(String.Empty, err.Description);
        }
    }
}