using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Identity.Extensions
{
    public static class IUrlHelperExtensions
    {
        /// <summary>
        /// Returns a link to a password reset page based on the given arguments.
        /// <summary>
        /// <param name="tkn">The UserManager generated PasswordResetToken.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        public static string ResetPasswordPageLink(this IUrlHelper urlHelper, string userId, string tkn,
            string protocol)
        {
            return urlHelper.Page(
                "/account/resetpassword",
                pageHandler: null,
                // UserId value prevents manual entering of email at resetpass form
                values: new { userId = userId, code = tkn },
                protocol: protocol);
        }

        /// <summary>
        /// Returns a link to an email confirmation page based on the given arguments.
        /// <summary>
        /// <param name="tkn">The UserManager generated EmailConfirmationToken.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        public static string ConfirmEmailPageLink(this IUrlHelper urlHelper, string userId, string tkn,
            string protocol)
        {
            return urlHelper.Page(
                "/account/confirmemail",
                pageHandler: null,
                values: new { userId = userId, code = tkn },
                protocol: protocol);
        }

        /// <summary>
        /// Returns a link to an email change page based on the given arguments.
        /// <summary>
        /// <param name="tkn">The UserManager generated EmailChangeToken.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        public static string ChangeEmailPageLink(this IUrlHelper urlHelper, string userId, string tkn,
            string newEmail, string protocol)
        {
            return urlHelper.Page(
            "/account/changeemailconfirmation",
            pageHandler: null,
            values: new { userId = userId, email = newEmail, code = tkn },
            protocol: protocol);
        }
    }
}