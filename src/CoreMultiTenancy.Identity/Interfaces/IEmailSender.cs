namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IEmailSender
    {
        Task SendAccountConfirmationEmail(string email, string callbackUrl);
        Task SendPasswordResetEmail(string email, string callbackUrl);
        Task SendEmailChangeEmail(string email, string callbackUrl);
        Task SendUnverifiedPassResetEmail(string email, string callbackUrl);
    }
}