namespace CoreMultiTenancy.Identity.ViewModels.Account
{
    public class LogoutViewModel
    {
        public string LogoutId { get; set; }
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}