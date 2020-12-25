using Grpc.Net.Client;

namespace CoreMultiTenancy.Api.Grpc
{
    public class AuthzChannelService : IAuthzChannelService
    {
        private readonly GrpcChannel _channel;
        public AuthzChannelService() => _channel = GrpcChannel.ForAddress("https://localhost:5100");

        public GrpcChannel CurrentChannel() => _channel;

        ~AuthzChannelService() => _channel.Dispose();
    }
}