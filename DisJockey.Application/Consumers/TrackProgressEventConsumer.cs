using DisJockey.Application.Contracts;
using DisJockey.Shared.Messaging.Events;
using DisJockey.Shared.Notifications;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class TrackProgressEventConsumer
{
    private readonly ILogger<TrackProgressEventConsumer> _logger;
    private readonly INotifier _notifier;

    public TrackProgressEventConsumer(
        ILogger<TrackProgressEventConsumer> logger,
        INotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;
    }

    public async Task Consume(TrackProgressEvent trackProgressEvent)
    {
        _logger.LogDebug(
            "Received {EventName} for VoiceChannel {VoiceChannelId}",
            nameof(TrackProgressEvent),
            trackProgressEvent.VoiceChannelId);

        var trackProgressNotification = new TrackProgressNotification(trackProgressEvent.ElapsedSeconds, trackProgressEvent.TotalSeconds);

        await _notifier.SendTrackProgressNotificationAsync(
            trackProgressEvent.VoiceChannelId,
            trackProgressNotification);
    }
}
