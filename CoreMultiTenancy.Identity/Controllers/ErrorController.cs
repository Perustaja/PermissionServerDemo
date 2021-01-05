using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Identity.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/api/error")]
        public IActionResult Error() => Problem();
    }
}