using CoreMultiTenancy.Identity.Authorization;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IAuthorizationEvaluator
    {
        Task<AuthorizeDecision> EvaluateAsync(string userId, string orgId, params string[] perms);
    }
}