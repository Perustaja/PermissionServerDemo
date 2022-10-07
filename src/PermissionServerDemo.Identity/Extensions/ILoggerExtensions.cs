using PermissionServerDemo.Identity.Entities;

namespace PermissionServerDemo.Identity.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogEmptyAuthenticatedUser(this ILogger logger, User user)
        {
            logger.LogWarning($"User authenticated but User object returned was null. User Id: {user?.Id}");
        }
    }
}