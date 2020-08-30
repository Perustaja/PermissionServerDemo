using Microsoft.AspNetCore.Mvc.Razor;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class RazorPageBaseExtensions
    {
        /// <summary>
        /// Checks the current ViewData dictionary for a SuccessMessage key with accompanying data.
        /// </summary>
        public static bool SuccessMessageViewDataExists(this RazorPageBase page)
            => page.ViewContext.ViewData["SuccessMessage"] != null;
    }
}