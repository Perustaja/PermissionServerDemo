using Grpc.Net.Client;

namespace CoreMultiTenancy.Api.Grpc
{
    /// <summary>
    /// Allows a channel to be reused across authorization calls.
    /// </summary>
    public interface IAuthzChannelService
    {
        /// <summary></summary>
        /// <returns>The current channel to the authorization service.</returns>
        GrpcChannel CurrentChannel();
    }
}