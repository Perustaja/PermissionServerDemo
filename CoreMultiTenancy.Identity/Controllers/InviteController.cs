using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    public class InviteController : Controller
    {
        private readonly string _existingAccessMessage = "This account already has access to this Organization.";
        private readonly string _successMessage = "Your account now has access to the specified Organization.";
        private readonly ILogger<InviteController> _logger;
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly IOrganizationInviteCodeService _codeService;
        public InviteController(ILogger<InviteController> logger,
            IUserOrganizationRepository userOrgRepo,
            IOrganizationInviteCodeService codeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
        }
        // GET Invite/{inviteCode}
        [HttpGet]
        [Route("Invite/{inviteCode}")]
        public async Task<IActionResult> Index(string inviteCode)
        {
            TempData["Success"] = false;
            var codeResult = _codeService.DecodeInvitation(inviteCode);
            if (!String.IsNullOrEmpty(codeResult.ErrorMessage))
            {
                TempData["Message"] = codeResult.ErrorMessage;
                return View();
            }

            if (User?.Identity.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null && Guid.TryParse(userId, out var parsedId))
                {
                    // Return view with error message if User already has access
                    if (await _userOrgRepo.UserHasAccess(parsedId, codeResult.DecodedValue))
                    {
                        TempData["Message"] = _existingAccessMessage;
                        return View();
                    }
                    // If not, add access
                    try
                    {
                        await _userOrgRepo.AddAsync(new UserOrganization(parsedId, codeResult.DecodedValue));
                        TempData["Success"] = true;
                        TempData["Message"] = _successMessage;
                        return View();
                    }
                    catch
                    {
                        _logger.LogError($"Exception encountered while trying to add Organization access. User: {parsedId} Org: {codeResult.DecodedValue}.");
                        return View("Error");
                    }
                }
                // Unable to parse NameIdentifier into GUID, return error
                return View("Error");
            }
            // Redirect to login
            return RedirectToAction("Login", "Account", new { ReturnUrl = $"/Invite/{inviteCode}" });
        }
    }
}