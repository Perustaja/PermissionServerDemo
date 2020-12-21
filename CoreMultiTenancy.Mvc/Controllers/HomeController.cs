using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace CoreMultiTenancy.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("/")]
        public async Task<IActionResult> CallApi()
        {
            // Get access token and display information
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            _logger.LogInformation(accessToken);
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var result  = await client.GetFromJsonAsync<string>("https://localhost:6100/identity"); // Call API

            ViewBag.Json = result;
            return View("Json");
        }

        [Route("/aircraft")]
        public async Task<IActionResult> CallTenantedApi()
        {
            // This represents a generic tenant id that the client would receive via a client-side "portal"
            // the client would track this locally and use it to construct dynamic api requests per tenant.
            string tid = "955f2c27-b860-4d23-9baf-383ccb48555a";
            // Get access token and display information
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            _logger.LogInformation(accessToken);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var url = HtmlEncoder.Default.Encode($"https://localhost:6100/api/{tid}/aircraft");
            var result = await client.GetFromJsonAsync<object>(url);

            ViewBag.Json = result;
            return View("Json");
        }
    }
}
