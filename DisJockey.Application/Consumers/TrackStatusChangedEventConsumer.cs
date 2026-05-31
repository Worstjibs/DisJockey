using DisJockey.Application.Contracts;
using DisJockey.Shared.Messaging.Events;
using DisJockey.Shared.Notifications;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class TrackStatusChangedEventConsumer
{
    private readonly ILogger<TrackStatusChangedEventConsumer> _logger;
    private readonly INotifier _notifier;

    public TrackStatusChangedEventConsumer(
        ILogger<TrackStatusChangedEventConsumer> logger,
        INotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;
    }

    public async Task Consume(TrackStatusChangedEvent trackStatusChangedEvent)
    {
        _logger.LogDiscordEventReceived(
            nameof(TrackStatusChangedEvent),
            trackStatusChangedEvent.VoiceChannelId);

        TrackStatusNotification? message = null;
        if (trackStatusChangedEvent.TrackDetails is not null)
        {
            message = new TrackStatusNotification(trackStatusChangedEvent.TrackDetails.TrackName, Paused: false); // TODO: Fix Paused
        }

        await _notifier.SendTrackNotificationAsync(trackStatusChangedEvent.VoiceChannelId, message);
    }
}
