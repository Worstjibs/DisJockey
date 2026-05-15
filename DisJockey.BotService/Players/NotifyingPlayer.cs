using DisJockey.BotService.Hubs;
using DisJockey.Shared.Notifications;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;

namespace DisJockey.BotService.Players;

internal class NotifyingPlayer : QueuedLavalinkPlayer
{
    private readonly HubConnectionProvider _hubConnectionProvider;
    private CancellationTokenSource? _trackTimerCts;

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

        _trackTimerCts = new CancellationTokenSource();
        _ = RunTrackTimerAsync(_trackTimerCts.Token);

        var notification = new TrackStatusChangedNotification(VoiceChannelId, new(trackName));

        await _hubConnectionProvider.InvokeAsync("TrackStatusChanged", notification);
    }

    protected override async ValueTask NotifyTrackEndedAsync(
        ITrackQueueItem queueItem,
        TrackEndReason endReason,
        CancellationToken cancellationToken = default)
    {
        await CancelTrackTimerAsync();

        if (endReason is not TrackEndReason.Stopped and not TrackEndReason.Finished)
        {
            return;
        }

        var notification = new TrackStatusChangedNotification(VoiceChannelId);

        await _hubConnectionProvider.InvokeAsync("TrackStatusChanged", notification);
    }

    private async Task RunTrackTimerAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            if (State is not PlayerState.Playing)
            {
                continue;
            }

            var elapsed = (int)Position!.Value.Position.TotalSeconds;
            var total = (int)CurrentTrack!.Duration.TotalSeconds;

            var notification = new TrackProgressNotification(VoiceChannelId, elapsed, total);
            await _hubConnectionProvider.InvokeAsync("PublishTrackProgress", notification);
        }
    }

    private async Task CancelTrackTimerAsync()
    {
        if (_trackTimerCts is null)
        {
            return;
        }

        await _trackTimerCts.CancelAsync();
        _trackTimerCts.Dispose();
        _trackTimerCts = null;
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
