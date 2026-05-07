using DisJockey.Shared.Protos;
using Grpc.Core;

namespace DisJockey.BotService.Grpc;

public class DisJockeyGrpcService : DisJockeyGrpc.DisJockeyGrpcBase
{
    public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PingResponse { Message = "Pong from BotService" });
    }
}
