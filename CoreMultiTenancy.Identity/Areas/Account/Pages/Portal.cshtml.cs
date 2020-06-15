using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoreMultiTenancy.Identity.Areas.Account.Pages
{
    public class PortalModel : PageModel
    {
        public static readonly string CookieName = Constants.TenantCookieName;
        public PortalModel()
        {
            // Use some tenant provider to get all current tenants or just call db ourselves
        }
        public List<Guid> OrganizationIds { get; set; }
        public List<string> OrganizationTitles { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // List organizations this user has access to.
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Set cookie based on their selection
            // Reroute to login or reroute to endpoint
            return Page();
        }
    }
}