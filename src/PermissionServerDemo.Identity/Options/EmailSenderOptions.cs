namespace PermissionServerDemo.Identity.Options
{
    /// <summary>
    /// Configuration class for taking in SendGrid information requires to send emails. Variables are pulled
    /// from user-secrets (see README.md for further information).
    /// </summary>
    public class EmailSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}