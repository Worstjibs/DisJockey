using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Protocol.Payloads.Events;
using System.Diagnostics;

namespace DisJockey.BotService.Players;

internal class NotifyingPlayer : QueuedLavalinkPlayer
{
    private static readonly ActivitySource _activitySource = new("DisJockey.BotService.Players.NotifyingPlayer");

    private readonly IServiceScopeFactory _scopeFactory;
    private CancellationTokenSource? _trackTimerCts;

    private Activity? _trackActivity;

    public NotifyingPlayer(IPlayerProperties<NotifyingPlayer, QueuedLavalinkPlayerOptions> properties)
        : base(properties)
    {
        _scopeFactory = properties.ServiceProvider!.GetRequiredService<IServiceScopeFactory>();
    }

    protected override async ValueTask NotifyTrackStartedAsync(
        ITrackQueueItem track,
        CancellationToken cancellationToken = default)
    {
        _trackActivity = _activitySource.StartActivity("Play Track");

        var trackName = track.Track?.Title ?? string.Empty;

        _trackTimerCts = new CancellationTokenSource();
        _ = RunTrackTimerAsync(_trackTimerCts.Token);

        await PublishEventAsync(new TrackStatusChangedEvent(VoiceChannelId, new TrackDetails(trackName)));
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

        await PublishEventAsync(new TrackStatusChangedEvent(VoiceChannelId, null));

        _trackActivity?.Dispose();
        _trackActivity = null;
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

            await PublishEventAsync(new TrackProgressEvent(VoiceChannelId, elapsed, total));
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

    private async Task PublishEventAsync<T>(T @event)
    {
        using var scope = _scopeFactory.CreateScope();
        var messageSender = scope.ServiceProvider.GetRequiredService<IMessageSender>();
        await messageSender.SendAsync(@event);
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
