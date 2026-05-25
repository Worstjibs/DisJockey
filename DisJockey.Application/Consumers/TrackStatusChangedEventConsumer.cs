using DisJockey.Application.Hubs;
using DisJockey.Shared.Messaging.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class TrackStatusChangedEventConsumer
{
    private readonly ILogger<TrackStatusChangedEventConsumer> _logger;
    private readonly IHubContext<TrackControlHub, ITrackControlHub> _hubContext;

    public TrackStatusChangedEventConsumer(
        ILogger<TrackStatusChangedEventConsumer> logger,
        IHubContext<TrackControlHub, ITrackControlHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Consume(TrackStatusChangedEvent trackStatusChangedEvent)
    {
        _logger.LogDebug(
            "Received {EventName} for VoiceChannel {VoiceChannelId}",
            nameof(TrackStatusChangedEvent),
            trackStatusChangedEvent.VoiceChannelId);

        TrackStatusMessage? message = null;
        if (trackStatusChangedEvent.TrackDetails is not null)
        {
            message = new TrackStatusMessage(trackStatusChangedEvent.TrackDetails.TrackName, Paused: false); // TODO: Fix Paused
        }

        await _hubContext.Clients.SendTrackNotificationAsync(trackStatusChangedEvent.VoiceChannelId, message);
    }
}
