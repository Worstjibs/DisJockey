using DisJockey.Application.Hubs;
using DisJockey.Shared.Messaging.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public class TrackProgressEventConsumer
{
    private readonly ILogger<TrackProgressEventConsumer> _logger;
    private readonly IHubContext<TrackControlHub, ITrackControlHub> _hubContext;

    public TrackProgressEventConsumer(
        ILogger<TrackProgressEventConsumer> logger,
        IHubContext<TrackControlHub, ITrackControlHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Consume(TrackProgressEvent trackProgressEvent)
    {
        _logger.LogDebug(
            "Received {EventName} for VoiceChannel {VoiceChannelId}",
            nameof(TrackProgressEvent),
            trackProgressEvent.VoiceChannelId);

        await _hubContext.Clients.SendTrackProgressNotificationAsync(
            trackProgressEvent.VoiceChannelId,
            new TrackProgressMessage(trackProgressEvent.ElapsedSeconds, trackProgressEvent.TotalSeconds));
    }
}
