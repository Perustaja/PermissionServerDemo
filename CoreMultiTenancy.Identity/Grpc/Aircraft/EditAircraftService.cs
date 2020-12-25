using System.Threading.Tasks;
using Grpc.Core;
using Cmt.Protobuf;

namespace CoreMultiTenancy.Identity.Grpc.Aircraft
{
    public class EditAircraftService : EditAircraft.EditAircraftBase
    {
        public override async Task<AuthorizeDecision> EditAircraft(EditAircraftRequest req, ServerCallContext ctx)
        {
            return await Task.FromResult(new AuthorizeDecision{ Allowed = true, Message = "Hello world!" });
        }
    }
}