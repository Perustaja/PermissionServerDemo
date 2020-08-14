using System.Threading.Tasks;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IEmailSender
    {
        Task SendAccountConfirmationEmail(string email, string confirmationUrl);
        Task SendPasswordResetEmail(string email, string resetUrl);
        Task SendEmailChangeEmail(string email, string token);
    }
}