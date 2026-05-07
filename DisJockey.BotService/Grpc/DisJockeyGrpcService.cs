using DisJockey.BotService.Services;
using DisJockey.Shared.Protos;
using Grpc.Core;

namespace DisJockey.BotService.Grpc;

public class DisJockeyGrpcService : DisJockeyGrpc.DisJockeyGrpcBase
{
    private readonly IUserVoiceStateService _userVoiceStateService;

    public DisJockeyGrpcService(IUserVoiceStateService userVoiceStateService)
    {
        _userVoiceStateService = userVoiceStateService;
    }

    public override async Task<GetUserVoiceChannelResponse> GetUserVoiceChannel(
        GetUserVoiceChannelRequest request, 
        ServerCallContext context)
    {
        var result = await _userVoiceStateService.GetUserVoiceStateAsync(request.UserId, context.CancellationToken)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "User not found or not in a voice channel"));

        return new()
        {
            ServerId = result.ServerId,
            ServerName = result.ServerName,
            VoiceChannelId = result.VoiceChannelId,
            VoiceChannelName = result.VoiceChannelName
        };
    }
}
