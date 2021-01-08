using System.Threading.Tasks;
using Cmt.Protobuf;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IRemoteAuthorizationEvaluator
    {
        Task<AuthorizeDecision> EvaluateAsync(string userId, string orgId, params string[] perms);
    }
}