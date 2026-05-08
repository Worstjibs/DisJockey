using DisJockey.BotService.Hubs;
using DisJockey.Shared.Notifications;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

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
        var notification = new TrackStatusChangedNotification(track.Track?.Title ?? string.Empty);

        await _hubConnectionProvider.InvokeAsync("TrackStatusChanged", notification);

        await base.NotifyTrackStartedAsync(track, cancellationToken);
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