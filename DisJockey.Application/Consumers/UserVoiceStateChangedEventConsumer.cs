using DisJockey.Application.Hubs;
using DisJockey.Shared.Messaging.Events;
using DisJockey.Shared.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class UserVoiceStateChangedEventConsumer
{
    private readonly ILogger<UserVoiceStateChangedEventConsumer> _logger;
    private readonly IHubContext<TrackControlHub, ITrackControlHub> _hubContext;
    private readonly IUserConnectionTracker _userConnectionTracker;
    private readonly DisJockeyGrpc.DisJockeyGrpcClient _disJockeyGrpcClient;

    public UserVoiceStateChangedEventConsumer(
        ILogger<UserVoiceStateChangedEventConsumer> logger,
        IHubContext<TrackControlHub, ITrackControlHub> hubContext,
        IUserConnectionTracker userConnectionTracker,
        DisJockeyGrpc.DisJockeyGrpcClient disJockeyGrpcClient)
    {
        _logger = logger;
        _hubContext = hubContext;
        _userConnectionTracker = userConnectionTracker;
        _disJockeyGrpcClient = disJockeyGrpcClient;
    }

    public async Task Consume(UserVoiceStateChangedEvent userVoiceStateChangedEvent)
    {
        _logger.LogDebug(
            "Received {EventName} for Discord user {DiscordId}",
            nameof(UserVoiceStateChangedEvent),
            userVoiceStateChangedEvent.DiscordId);

        var userConnected = false;

        var voiceChannel = userVoiceStateChangedEvent.VoiceChannelDetails;
        if (voiceChannel is not null)
        {
            userConnected = true;

            var userConnectionId = _userConnectionTracker.GetConnection(userVoiceStateChangedEvent.DiscordId.ToString());
            if (userConnectionId is not null)
            {   
                await _hubContext.Groups.AddToGroupAsync(userConnectionId, voiceChannel.VoiceChannelId.ToString());
            }
        }

        await HandleVoiceStateVoiceStateEvent(userVoiceStateChangedEvent, userConnected);
    }

    private async Task HandleVoiceStateVoiceStateEvent(UserVoiceStateChangedEvent notification, bool connected)
    {
        if (!connected)
        {
            await _hubContext.Clients.SendUserStatusMessageAsync(notification.DiscordId, null);
            return;
        }

        var voiceChannel = notification.VoiceChannelDetails;
        if (voiceChannel is null)
        {
            return;
        }

        var trackStatus = await GetCurrentTrackStatusAsync(voiceChannel.ServerId, voiceChannel.VoiceChannelId);

        TrackStatusMessage? trackStatusMessage = null;
        if (trackStatus is not null)
        {
            trackStatusMessage = new TrackStatusMessage(trackStatus.TrackName, trackStatus.Paused);
        }

        var userStatusMessage = new UserStatusMessage(
            voiceChannel.VoiceChannelName,
            voiceChannel.ServerName,
            trackStatusMessage);

        await _hubContext.Clients.SendUserStatusMessageAsync(notification.DiscordId, userStatusMessage);
    }

    private async Task<TrackStatusMessage?> GetCurrentTrackStatusAsync(
        ulong serverId,
        ulong voiceChannelId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var trackStatus = await _disJockeyGrpcClient.GetTrackStatusAsync(new()
            {
                ServerId = serverId,
                VoiceChannelId = voiceChannelId
            }, cancellationToken: cancellationToken);

            return new TrackStatusMessage(trackStatus.TrackName, trackStatus.Paused);
        }
        catch (RpcException e) when (e.StatusCode is StatusCode.NotFound)
        {
            return null;
        }
    }
}
