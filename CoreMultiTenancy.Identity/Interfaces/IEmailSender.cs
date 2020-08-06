using System.Threading.Tasks;

namespace CoreMultiTenancy.Identity.Interfaces
{
    interface IEmailSender
    {
        Task SendAccountConfirmationEmail(string email, string confirmationUrl);
        Task SendPasswordResetEmail(string email, string resetUrl);
    }
}