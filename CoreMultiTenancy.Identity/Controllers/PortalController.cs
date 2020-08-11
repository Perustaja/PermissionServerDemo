using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.ViewModels.Portal;
using CoreMultiTenancy.Identity.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    [SecurityHeaders]
    [Authorize]
    public class PortalController : Controller
    {
        private readonly ILogger<PortalController> _logger;
        private readonly IMapper _mapper;
        private readonly IOrganizationRepository _orgRepository;
        public PortalController(ILogger<PortalController> logger,
            IMapper mapper, IOrganizationRepository orgRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _orgRepository = orgRepository ?? throw new ArgumentNullException(nameof(orgRepository));
        }
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                ?? throw new Exception("Unable to find subject claim.");
            if (!Guid.TryParse(userId, out var parsedId))
                throw new ArgumentException("Unable to parse subject's id to valid Guid.");
            
            var validOrgs = await _orgRepository.GetUsersOrgsById(parsedId);
            var orgVms = _mapper.Map<List<OrganizationViewModel>>(validOrgs);
            var vm = new SelectOrganizationViewModel() { OrganizationViewModels = orgVms };
            vm.ReturnUrl = returnUrl;

            ViewData["ReturnUrl"] = returnUrl;
            // NOTE: ensure viewdata["errormessage"] is displayed with redirect and set in responsegen
            return View(vm);
        }
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult SelectOrganization(Guid orgId)
        // {
        //     // Validate selectedorg
        //     // Set user's selectedorg
        //     // redirect to returnUrl
        // }
    }
}