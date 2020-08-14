using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IActionResult Index()
        {
            var handler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            // Log error if redirected here
            if (handler?.Error != null)
                _logger.LogError($"Exception page invoked. Path: {handler?.Path} Error: {handler?.Error.Message}");

            return View();
        }
        [Route("/notfound")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}