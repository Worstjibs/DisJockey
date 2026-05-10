using DisJockey.BotService.Hubs;
using DisJockey.Shared.Notifications;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;

namespace DisJockey.BotService.Players;

internal class NotifyingPlayer : QueuedLavalinkPlayer
{
    private readonly HubConnectionProvider _hubConnectionProvider;

    public NotifyingPlayer(IPlayerProperties<NotifyingPlayer, QueuedLavalinkPlayerOptions> properties)
        : base(properties)
    {
        _hubConnectionProvider = properties.ServiceProvider!.GetRequiredService<HubConnectionProvider>();
    }

    protected override async ValueTask NotifyTrackStartedAsync(
        ITrackQueueItem track,
        CancellationToken cancellationToken = default)
    {
        var trackName = track.Track?.Title ?? string.Empty;

        var notification = new TrackStatusChangedNotification(VoiceChannelId, new(trackName));

        await _hubConnectionProvider.InvokeAsync("TrackStatusChanged", notification);
    }

    protected override async ValueTask NotifyTrackEndedAsync(
        ITrackQueueItem queueItem, 
        TrackEndReason endReason, 
        CancellationToken cancellationToken = default)
    {
        if (endReason is not TrackEndReason.Stopped)
        {
            return;
        }

        var notification = new TrackStatusChangedNotification(VoiceChannelId);

        await _hubConnectionProvider.InvokeAsync("TrackStatusChanged", notification);
    }

   internal static ValueTask<NotifyingPlayer> CreatePlayerAsync(
        IPlayerProperties<NotifyingPlayer, QueuedLavalinkPlayerOptions> properties, 
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(properties);

        return ValueTask.FromResult(new NotifyingPlayer(properties));
    }
}