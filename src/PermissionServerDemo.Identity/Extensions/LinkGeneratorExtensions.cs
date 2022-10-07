namespace PermissionServerDemo.Identity.Extensions
{
    public static class LinkGeneratorExtensions
    {
        /// <summary>
        /// Returns a link to a password reset page based on the given arguments.
        /// <summary>
        /// <param name="tkn">The UserManager generated PasswordResetToken.</param>
        public static string ResetPasswordPageLink(this LinkGenerator gen, HttpContext cont, 
            string userId, string tkn)
        {
            return gen.GetUriByPage(cont,
                page: "/account/resetpassword",
                // UserId value prevents manual entering of email at resetpass form
                values: new { userId = userId, code = tkn }
                );
        }

        /// <summary>
        /// Returns a link to an email confirmation page based on the given arguments.
        /// <summary>
        /// <param name="tkn">The UserManager generated EmailConfirmationToken.</param>
        public static string ConfirmEmailPageLink(this LinkGenerator gen, HttpContext cont,
            string userId, string tkn)
        {
            return gen.GetUriByPage(cont,
                page: "/account/confirmemail",
                values: new { userId = userId, code = tkn }
            );
        }

        /// <summary>
        /// Returns a link to an email change page based on the given arguments.
        /// <summary>
        /// <param name="tkn">The UserManager generated EmailChangeToken.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        public static string ChangeEmailPageLink(this LinkGenerator gen, HttpContext cont,
            string userId, string tkn, string newEmail)
        {
            return gen.GetUriByPage(cont,
            "/account/changeemailconfirmation",
            values: new { userId = userId, email = newEmail, code = tkn }
            );
        }
    }
}