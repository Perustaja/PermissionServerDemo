namespace PermissionServerDemo.Identity.Extensions
{
    public static class IConfigurationExtensions
    {
        public static Guid GetDemoRoleId(this IConfiguration configuration, string name)
        {
            if (Guid.TryParse(configuration[name], out Guid res))
                return res;
            throw new Exception($"Location of {name} from appsettings.json not found or parsing to Guid failed.");
        }
    }
}