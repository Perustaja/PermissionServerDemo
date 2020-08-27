using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Pages.Account
{
    [AllowAnonymous]
    public class SentConfirmationEmailModel : PageModel
    {
        [TempData]
        public bool RedirectSuccess { get; set; }

        [TempData]
        public string RedirectResultMessage { get; set; }
        public IActionResult OnGet()
        {
            if (String.IsNullOrEmpty(RedirectResultMessage))
                return RedirectToPage("/home/index");
            return Page();
        }
    }
}