using Microsoft.AspNetCore.Mvc;

namespace PermissionServerDemo.Identity.Controllers
{
    public class ErrorController : ControllerBase
    {
        [Route("/api/error")]
        public IActionResult Error() => Problem();
    }
}