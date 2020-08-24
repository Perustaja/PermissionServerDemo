using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class PageModelExtensions
    {
        public static IActionResult LoadingPage(this PageModel page, string viewName, string redirectUri)
        {
            page.HttpContext.Response.StatusCode = 200;
            page.HttpContext.Response.Headers["Location"] = "";

            return page.RedirectToPage("redirect", new { RedirectUri = redirectUri });
        }
        public static void AddIdentityResultErrors(this PageModel page, IdentityResult result)
        {
            foreach (var err in result.Errors)
                page.ModelState.AddModelError(String.Empty, err.Description);
        }
    }
}