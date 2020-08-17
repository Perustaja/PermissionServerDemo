using System;
using System.Security.Policy;
using CoreMultiTenancy.Identity.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
        {
            controller.HttpContext.Response.StatusCode = 200;
            controller.HttpContext.Response.Headers["Location"] = "";

            return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
        }
        public static void AddIdentityResultErrors(this Controller controller, IdentityResult result)
        {
            foreach (var err in result.Errors)
                controller.ModelState.AddModelError(String.Empty, err.Description);
        }
    }
}