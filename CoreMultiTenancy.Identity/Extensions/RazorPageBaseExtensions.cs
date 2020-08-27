using Microsoft.AspNetCore.Mvc.Razor;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class RazorPageBaseExtensions
    {
        /// <summary>
        /// Checks the current ViewData dictionary for Success and ResultMessage entries.
        /// </summary>
        public static bool ValidationMessageViewDataExists(this RazorPageBase page)
        {
            if (page.ViewContext.ViewData["Success"] != null && page.ViewContext.ViewData["ResultMessage"] != null)
                return true;
            return false;
        }
    }
}