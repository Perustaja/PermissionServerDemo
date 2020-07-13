using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using System;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Xml;

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

        public IActionResult Index()
        {
            return View();
        }
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
    }
}
