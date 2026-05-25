using DisJockey.Application.Clients;
using DisJockey.Application.Hubs;
using DisJockey.Shared.Messaging.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class UserVoiceStateChangedEventConsumer
{
    private readonly ILogger<UserVoiceStateChangedEventConsumer> _logger;
    private readonly IHubContext<TrackControlHub, ITrackControlHub> _hubContext;
    private readonly IUserConnectionTracker _userConnectionTracker;
    private readonly IBotServiceClient _botServiceClient;

    public UserVoiceStateChangedEventConsumer(
        ILogger<UserVoiceStateChangedEventConsumer> logger,
        IHubContext<TrackControlHub, ITrackControlHub> hubContext,
        IUserConnectionTracker userConnectionTracker,
        IBotServiceClient botServiceClient)
    {
        _logger = logger;
        _hubContext = hubContext;
        _userConnectionTracker = userConnectionTracker;
        _botServiceClient = botServiceClient;
    }

    public async Task Consume(UserVoiceStateChangedEvent userVoiceStateChangedEvent)
    {
        _logger.LogDebug(
            "Received {EventName} for Discord user {DiscordId}",
            nameof(UserVoiceStateChangedEvent),
            userVoiceStateChangedEvent.DiscordId);

        var voiceChannel = userVoiceStateChangedEvent.VoiceChannelDetails;
        if (voiceChannel is not null)
        {
            var userConnectionId = _userConnectionTracker.GetConnection(userVoiceStateChangedEvent.DiscordId.ToString());
            if (userConnectionId is not null)
            {
                await _hubContext.Groups.AddToGroupAsync(userConnectionId, voiceChannel.VoiceChannelId.ToString());
            }

            var trackStatus = await _botServiceClient.GetTrackStatusAsync(voiceChannel.ServerId, voiceChannel.VoiceChannelId);

            var trackStatusMessage = trackStatus is not null
                                        ? new TrackStatusMessage(trackStatus.TrackName, trackStatus.Paused)
                                        : null;

            var userStatusMessage = new UserStatusMessage(
                voiceChannel.VoiceChannelName,
                voiceChannel.ServerName,
                trackStatusMessage);

            await _hubContext.Clients.SendUserStatusMessageAsync(userVoiceStateChangedEvent.DiscordId, userStatusMessage);
        }
        else
        {
            await _hubContext.Clients.SendUserStatusMessageAsync(userVoiceStateChangedEvent.DiscordId, null);
        }
    }
}
