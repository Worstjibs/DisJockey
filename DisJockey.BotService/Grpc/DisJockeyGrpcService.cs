using DisJockey.BotService.Services;
using DisJockey.BotService.Services.Music;
using DisJockey.Shared.Protos;
using Grpc.Core;

namespace DisJockey.BotService.Grpc;

public class DisJockeyGrpcService : DisJockeyGrpc.DisJockeyGrpcBase
{
    private readonly IUserVoiceStateService _userVoiceStateService;
    private readonly IMusicService _musicService;

    public DisJockeyGrpcService(
        IUserVoiceStateService userVoiceStateService,
        IMusicService musicService)
    {
        _userVoiceStateService = userVoiceStateService;
        _musicService = musicService;
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

    public override async Task<GetTrackStatusResponse> GetTrackStatus(GetTrackStatusRequest request, ServerCallContext context)
    {
        var player = await _musicService.GetQueuedLavalinkPlayerAsync(
                request.ServerId, 
                request.VoiceChannelId, 
                connectToVoiceChannel: false) 
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Player not found"));

        var track = player.CurrentTrack ?? throw new RpcException(new Status(StatusCode.NotFound, "No track is currently playing"));

        return new GetTrackStatusResponse
        {
            TrackId = track.Identifier,
            TrackName = track.Title
        };
    }
}