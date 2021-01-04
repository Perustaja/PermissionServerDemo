using System;
namespace CoreMultiTenancy.Identity.Options
{
    /// <summary>
    /// Oidc specific account options used as defaults. External idps take precedence.
    /// </summary>
    public class OidcAccountOptions
    {
        public bool AllowLocalLogin { get; set; }
        public bool AllowRememberLogin { get; set; }
        public TimeSpan RememberMeLoginDuration { get; set; }
        public bool ShowLogoutPrompt { get; set; }
        public bool AutomaticRedirectAfterSignOut { get; set; }
        public string InvalidCredentialsErrorMessage { get; set; }
    }
}
