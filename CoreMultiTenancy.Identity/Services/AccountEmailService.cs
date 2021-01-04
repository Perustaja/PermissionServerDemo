using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Services
{
    public class AccountEmailService : IAccountEmailService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly HttpContext _httpContext;
        private readonly LinkGenerator _linkGen;

        public AccountEmailService(UserManager<User> userManager, IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor, LinkGenerator linkGen)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _linkGen = linkGen ?? throw new ArgumentNullException(nameof(linkGen));
        }

        public async Task<Option<string>> SendConfToAuthUserAsync(User user)
        {
            if (user.EmailConfirmed)
                return Option<string>.Some("Your account's email is already confirmed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _linkGen.ConfirmEmailPageLink(_httpContext, user.Id.ToString(), token);
            await _emailSender.SendAccountConfirmationEmail(user.Email, callbackUrl);
            return Option<string>.None;
        }

        public async Task<Option<string>> SendConfToUnauthUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Do not notify an unauthenticated user whether or not an email is verified, so do 
                // nothing but return the same result if associated user's email has been confirmed.
                if (!user.EmailConfirmed)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = _linkGen.ConfirmEmailPageLink(_httpContext, user.Id.ToString(), token);
                    await _emailSender.SendAccountConfirmationEmail(user.Email, callbackUrl);
                }
                return Option<string>.None;
            }
            // The user can easily figure out if an email is registered by attempting to make an
            // account under it, so exposing this detail here is not a security concern.
            return Option<string>.Some("No account associated with the entered email exists.");
        }

        public async Task<Option<string>> SendEmailChangeEmail(string currEmail, string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(currEmail);
            if (user == null)
                return Option<string>.Some("No account associated with the entered email exists.");

            if (currEmail != newEmail)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
                var callbackUrl = _linkGen.ChangeEmailPageLink(_httpContext, user.Id.ToString(), token, newEmail);
                await _emailSender.SendEmailChangeEmail(newEmail, callbackUrl);
                return Option<string>.None;
            }
            return Option<string>.Some("New email cannot equal old email.");
        }

        public async Task SendPassResetEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // If email is confirmed, send email, if not send explanation email
                if (user.EmailConfirmed)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = _linkGen.ResetPasswordPageLink(_httpContext, user.Id.ToString(), token);
                    await _emailSender.SendPasswordResetEmail(user.Email, callbackUrl);
                }
                else
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = _linkGen.ConfirmEmailPageLink(_httpContext, user.Id.ToString(), token);
                    await _emailSender.SendUnverifiedPassResetEmail(user.Email, callbackUrl);
                }
            }
        }
    }
}