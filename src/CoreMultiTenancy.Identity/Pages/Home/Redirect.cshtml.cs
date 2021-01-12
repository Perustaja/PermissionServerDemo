using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Pages.Home
{
    public class RedirectModel : PageModel 
    {
        [ViewData]
        public string RedirectUrl { get; set; }
        public IActionResult OnGet(string redirectUri)
        {
            RedirectUrl = redirectUri;
            return Page();
        }
    }
}