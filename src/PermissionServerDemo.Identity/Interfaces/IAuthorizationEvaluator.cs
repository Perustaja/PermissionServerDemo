using PermissionServerDemo.Identity.Authorization;

namespace PermissionServerDemo.Identity.Interfaces
{
    public interface IAuthorizationEvaluator
    {
        Task<AuthorizeDecision> EvaluateAsync(string userId, string orgId, params string[] perms);
    }
}