using DisJockey.Application.Clients;
using DisJockey.Application.Contracts;
using DisJockey.Shared.Messaging.Events;
using DisJockey.Shared.Notifications;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class UserVoiceStateChangedEventConsumer
{
    private readonly ILogger<UserVoiceStateChangedEventConsumer> _logger;
    private readonly INotifier _notifier;
    private readonly IBotServiceClient _botServiceClient;

    public UserVoiceStateChangedEventConsumer(
        ILogger<UserVoiceStateChangedEventConsumer> logger,
        INotifier notifier,
        IBotServiceClient botServiceClient)
    {
        _logger = logger;
        _notifier = notifier;
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
            await _notifier.UpdateUserConnectionVoiceChannelAsync(userVoiceStateChangedEvent.DiscordId, voiceChannel.VoiceChannelId);

            var trackStatus = await _botServiceClient.GetTrackStatusAsync(voiceChannel.ServerId, voiceChannel.VoiceChannelId);

            var trackStatusMessage = trackStatus is not null
                                        ? new TrackStatusNotification(trackStatus.TrackName, trackStatus.Paused)
                                        : null;

            var userStatusMessage = new UserStatusNotification(
                voiceChannel.VoiceChannelName,
                voiceChannel.ServerName,
                trackStatusMessage);

            await _notifier.SendUserStatusMessageAsync(userVoiceStateChangedEvent.DiscordId, userStatusMessage);
        }
        else
        {
            await _notifier.SendUserStatusMessageAsync(userVoiceStateChangedEvent.DiscordId, null);
        }
    }
}
