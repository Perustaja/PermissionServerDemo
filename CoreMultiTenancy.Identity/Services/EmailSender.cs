using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CoreMultiTenancy.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSenderOptions _options;
        public EmailSender(ILogger<EmailSender> logger,
            IOptions<EmailSenderOptions> optionsAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            if (_options.SendGridKey == null || _options.SendGridUser == null)
                throw new ArgumentNullException($"{nameof(EmailSender)}: SendGridKey or SendGridUser not set on launch. Please configure.");
        }
        public async Task SendAccountConfirmationEmail(string email, string callbackUrl)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Account verification required",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                HtmlContent = $"Please confirm your account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>."
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            var response = await client.SendEmailAsync(msg);
            _logger.LogInformation($"Account confirmation email sent to {email}.");
        }

        public async Task SendPasswordResetEmail(string email, string callbackUrl)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Reset your password",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                HtmlContent = $"Reset your account's password by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>."
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
            _logger.LogInformation($"Password reset email sent to {email}.");
        }

        public async Task SendEmailChangeEmail(string email, string callbackUrl)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Change email",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                HtmlContent = $"Update your account's email address by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>."
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
            _logger.LogInformation($"Email change email sent to {email}.");
        }

        public async Task SendUnverifiedPassResetEmail(string email, string callbackUrl)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Password Reset",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                HtmlContent = $"A password reset request was made for the account associated with this email. However, your account must be verified. First, verify by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>, then make another password reset request."
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
            _logger.LogInformation($"User associated with {email} attempted to reset password without verification. Sent explanation email.");
        }
    }
}