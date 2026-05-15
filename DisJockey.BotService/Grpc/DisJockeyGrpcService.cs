using DisJockey.BotService.Services;
using DisJockey.BotService.Services.Music;
using DisJockey.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace DisJockey.BotService.Grpc;

public class DisJockeyGrpcService : DisJockeyGrpc.DisJockeyGrpcBase
{
    private readonly IUserVoiceStateService _userVoiceStateService;
    private readonly IMusicService _musicService;
    private readonly ILogger<DisJockeyGrpcService> _logger;

    public DisJockeyGrpcService(
        IUserVoiceStateService userVoiceStateService,
        IMusicService musicService,
        ILogger<DisJockeyGrpcService> logger)
    {
        _userVoiceStateService = userVoiceStateService;
        _musicService = musicService;
        _logger = logger;
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

        if (player.VoiceChannelId != request.VoiceChannelId)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Player does not match current user voice channel"));
        }

        var track = player.CurrentTrack ?? throw new RpcException(new Status(StatusCode.NotFound, "No track is currently playing"));

        return new GetTrackStatusResponse
        {
            TrackId = track.Identifier,
            TrackName = track.Title,
            Paused = player.IsPaused
        };
    }

    public override async Task<Empty> SeekTrack(SeekTrackRequest request, ServerCallContext context)
    {
        var player = await _musicService.GetQueuedLavalinkPlayerAsync(
                request.ServerId,
                request.VoiceChannelId,
                connectToVoiceChannel: false)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Player not found"));

        if (player.CurrentTrack is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No track is currently playing"));
        }

        await player.SeekAsync(TimeSpan.FromSeconds(request.PositionSeconds));

        return new();
    }

    public override async Task<Empty> PlayPauseTrack(PlayPauseTrackRequest request, ServerCallContext context)
    {
        var player = await _musicService.GetQueuedLavalinkPlayerAsync(
                request.ServerId,
                request.VoiceChannelId,
                connectToVoiceChannel: false)
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Player not found"));

        if (player.CurrentTrack is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No track is currently playing"));
        }

        if (!player.IsPaused)
        {
            await player.PauseAsync();

            _logger.LogPlayPausedState(
                player.SessionId,
                player.VoiceChannelId,
                player.GuildId,
                "Paused");
        }
        else
        {
            await player.ResumeAsync();

            _logger.LogPlayPausedState(
                player.SessionId,
                player.VoiceChannelId,
                player.GuildId,
                "Resumed");
        }

        return new();
    }
}