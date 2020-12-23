using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Extensions;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

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

        public async Task<AccountEmailResult> SendConfToAuthUserAsync(User user)
        {
            if (user != null)
            {
                if (user.EmailConfirmed)
                    return new AccountEmailResult(false, "Your account's email is already confirmed.");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = _linkGen.ConfirmEmailPageLink(_httpContext, user.Id.ToString(), token);
                await _emailSender.SendAccountConfirmationEmail(user.Email, callbackUrl);
                return new AccountEmailResult(true, $"A confirmation link has been sent to {user.Email}. You may need to check your spam folder.");
            }
            return new AccountEmailResult(false, "Unable to find user.");
        }

        public async Task<AccountEmailResult> SendConfToUnauthUserAsync(string email)
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
                return new AccountEmailResult(true, $"A confirmation link has been sent to {user.Email} if applicable. You may need to check your spam folder.");
            }
            // The user can easily figure out if an email is registered by attempting to make an
            // account under it, so exposing this detail here is not a security concern.
            return new AccountEmailResult(false, "No account associated with the entered email exists.");
        }

        public async Task<AccountEmailResult> SendEmailChangeEmail(string currEmail, string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(currEmail);
            if (user == null)
                return new AccountEmailResult(false, "Unable to find user.");
            
            if (currEmail != newEmail)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
                var callbackUrl = _linkGen.ChangeEmailPageLink(_httpContext, user.Id.ToString(), token, newEmail);
                await _emailSender.SendEmailChangeEmail(newEmail, callbackUrl);
                return new AccountEmailResult(true, $"An email containing a link to confirm your email change has been sent to {newEmail}.");
            }
            return new AccountEmailResult(false, "New email cannot equal old email.");
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