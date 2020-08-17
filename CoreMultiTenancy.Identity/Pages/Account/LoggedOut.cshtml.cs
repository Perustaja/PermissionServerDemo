using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoggedOutModel : PageModel
    {
        public IActionResult OnGet(LoggedOutViewModel vm)
        {
            return Page();
        }
    }
}