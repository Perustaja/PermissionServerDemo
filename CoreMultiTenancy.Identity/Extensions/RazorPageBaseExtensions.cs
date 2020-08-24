using System;
using Microsoft.AspNetCore.Mvc.Razor;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class RazorPageBaseExtensions
    {
        /// <summary>
        /// Checks the current TempData dictionary for Success and ResultMessage entries.
        /// </summary>
        public static bool ValidationMessageTempDataExists(this RazorPageBase page)
        {
            if (page.TempData.Peek("Success") != null && page.TempData.Peek("ResultMessage") != null)
                return true;
            return false;
        }
    }
}