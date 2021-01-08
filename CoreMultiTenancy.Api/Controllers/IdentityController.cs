using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace CoreMultiTenancy.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            // Todo: update to show tenant-specific stuff
            var claims = JsonConvert.SerializeObject(from c in User.Claims select new { c.Type, c.Value });
            var cookies = JsonConvert.SerializeObject(from c in HttpContext.Request.Cookies select new { c.Key, c.Value });
            return new JsonResult(claims + cookies);
        }
    }
}