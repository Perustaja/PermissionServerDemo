using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.ViewModels.Settings;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public SettingsController(ILogger<SettingsController> logger,
            UserManager<User> userManager,
            IMapper mapper,
            IEmailSender emailSender)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }
        // TempData for toast updates after form submission
        [TempData]
        public string ResultMessage { get; set; }
        [TempData]
        public bool Success { get; set; }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {

            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var vm = _mapper.Map<SettingsProfileViewModel>(user);
                return View(vm);
            }
            _logger.LogError($"{nameof(SettingsController)}: User authenticated but lookup returned null User object.");
            return RedirectToAction("Index", "Error");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(SettingsProfileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.UpdateName(vm.FirstName, vm.LastName);
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        ResultMessage = "Profile settings updated. Please allow up to an hour for changes to be reflected.";
                    else
                    {
                        ResultMessage = "There was an error updating your profile settings.";
                        _logger.LogError($"{nameof(SettingsController)}: Unable to update User profile information. FirstName: {vm.FirstName}, LastName: {vm.LastName}.");
                    }
                    Success = result.Succeeded;
                    return View();
                }
                _logger.LogError($"{nameof(SettingsController)}: User {user.Id} authenticated but lookup returned null User object.");
                return RedirectToAction("Index", "Error");
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Email()
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var vm = _mapper.Map<SettingsEmailViewModel>(user);
                return View(vm);
            }
            _logger.LogError($"{nameof(SettingsController)}: User authenticated but lookup returned null User object.");
            return RedirectToAction("Index", "Error");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Email(SettingsEmailViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    try
                    {
                        var token = await _userManager.GenerateChangeEmailTokenAsync(user, vm.NewEmail);
                        await _emailSender.SendEmailChangeEmail(vm.NewEmail, token);
                        Success = true;
                        ResultMessage = $"An email has been sent to {vm.NewEmail} with a confirmation link.";
                        return View();
                    }
                    catch
                    {
                        _logger.LogError($"{nameof(SettingsController)}: Email change token sending failed for User {user.Id} new email {vm.NewEmail}.");
                        return RedirectToAction("Index", "Error");
                    }
                }
            }
            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Password(SettingsPasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(JwtClaimTypes.Subject)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (user.NormalizedEmail == vm.Email.ToUpper() && await _userManager.CheckPasswordAsync(user, vm.ConfirmPassword))
                    {
                        try
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                            await _emailSender.SendPasswordResetEmail(user.Email, token);
                            Success = true;
                            ResultMessage = $"An email has been sent to {user.Email} with a password reset link.";
                            return View();
                        }
                        catch
                        {
                            _logger.LogError($"{nameof(SettingsController)}: Password reset token sending failed for User {user.Id}.");
                            return RedirectToAction("Index", "Error");
                        }
                    }
                    ModelState.AddModelError("AuthenticationFailed", "Invalid email or password.");
                    return View(vm);
                }
                _logger.LogError($"{nameof(SettingsController)}: User authenticated but lookup returned null User object.");
                return RedirectToAction("Index", "Error");
            }
            return View(vm);
        }
    }
}