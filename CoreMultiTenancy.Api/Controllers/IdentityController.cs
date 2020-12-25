using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace CoreMultiTenancy.Api.Controllers
{
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;
        public IdentityController(IHttpContextAccessor context)
        {
            _httpContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        public IActionResult Get()
        {
            // Todo: update to show tenant-specific stuff
            var claims = JsonConvert.SerializeObject(from c in User.Claims select new { c.Type, c.Value });
            var cookies = JsonConvert.SerializeObject(from c in _httpContext.HttpContext.Request.Cookies select new { c.Key, c.Value });
            return new JsonResult(claims + cookies);
        }
    }
}