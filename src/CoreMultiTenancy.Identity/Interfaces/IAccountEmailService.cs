using CoreMultiTenancy.Identity.Entities;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Interfaces
{
    /// <summary>
    /// Provides different email confirmation services based on the authentication state of the user.
    /// </summary>
    public interface IAccountEmailService
    {
        /// <summary>
        /// Attempts to send a confirmation email to the currently authenticated user.
        /// </summary>
        /// <returns>An Option containing a descriptive string on error.</returns>
        Task<Option<string>> SendConfToAuthUserAsync(User user);
        /// <summary>
        /// Attempts to send a confirmation email from some form entry accessible by anyone.
        /// </summary>
        /// <returns>An Option containing a descriptive string on error.</returns>
        Task<Option<string>> SendConfToUnauthUserAsync(string email);
        /// <summary>
        /// Sends an email with a link to change the user's current email.
        /// </summary>
        /// <returns>An Option containing a descriptive string on error.</returns>
        Task<Option<string>> SendEmailChangeEmail(string currEmail, string newEmail);
        /// <summary>
        /// Sends a password reset email to the user's email if the email is verified. If not, sends
        /// an email with further instructions on verifying their account.
        /// </summary>
        Task SendPassResetEmail(string email);
    }
}