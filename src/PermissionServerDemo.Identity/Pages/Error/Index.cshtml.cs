using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PermissionServerDemo.Identity.Pages.Error
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IActionResult OnGet()
        {
            var handler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            // Log error if redirected here
            if (handler?.Error != null)
                _logger.LogError($"Exception page invoked. Path: {handler?.Path} Error: {handler?.Error.Message}");

            return Page();
        }
    }
}