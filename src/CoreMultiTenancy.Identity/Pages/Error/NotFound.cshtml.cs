using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Pages.Error
{
    [AllowAnonymous]
    public class NotFoundModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}