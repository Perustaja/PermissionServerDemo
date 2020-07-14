# Setup
#### Email Configuration
Email configuration is stored in user secrets and injected via the IOptions<TOptions> interface.
To set the email configuration, sign up for a free account with SendGrid and enter your credentials into user secrets:
```
cd CoreMultiTenancy.Identity
dotnet user-secrets set "Email:SendGridUser" "<your_username>"
dotnet user-secrets set "Email:SendGridKey" "<your_key>"
```