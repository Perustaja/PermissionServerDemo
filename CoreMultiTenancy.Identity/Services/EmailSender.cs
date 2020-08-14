using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CoreMultiTenancy.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _options;
        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        }
        public async Task SendAccountConfirmationEmail(string email, string confirmationUrl)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Account verification required",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                PlainTextContent = $"Confirm your account by clicking the following link: {confirmationUrl}"
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetEmail(string email, string resetUrl)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Reset your password",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                PlainTextContent = $"Reset your account's password by using the following link: {resetUrl}."
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
        }

        public async Task SendEmailChangeEmail(string email, string token)
        {
            var client = new SendGridClient(_options.SendGridKey);
            var msg = new SendGridMessage()
            {
                Subject = "TestApp - Email change",
                From = new EmailAddress("no-reply@testapp.dev", _options.SendGridUser),
                PlainTextContent = $"Update your account's email address by clicking the following link: {token}."
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);
        }
    }
}