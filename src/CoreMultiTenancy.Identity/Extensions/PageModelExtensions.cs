using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class PageModelExtensions
    {
        /// <summary>
        /// Returns a redirect page for native clients that utilizes js/signin-redirect.
        /// </summary>
        public static IActionResult NativeRedirectPage(this PageModel page, string redirectUri)
        {
            page.HttpContext.Response.StatusCode = 200;
            page.HttpContext.Response.Headers["Location"] = "";

            return page.RedirectToPage("Redirect", new { RedirectUri = redirectUri });
        }

        /// <summary>
        /// Adds errors from Identityresult to the given page, to be displayed by model validation.
        /// </summary>
        /// <param name="page">Page whose ModelState is to be populated.</param>
        /// <param name="result">Result from an attempted identity operation.</param>
        public static void AddIdentityResultErrors(this PageModel page, IdentityResult result)
        {
            foreach (var err in result.Errors)
                page.ModelState.AddModelError(String.Empty, err.Description);
        }
    }
}