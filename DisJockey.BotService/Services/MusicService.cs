using Discord;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using Lavalink4NET.Tracks;
using DisJockey.BotService.Modules;
using System.Security;

namespace DisJockey.BotService.Services;

public class MusicService : IMusicService
{
    private readonly IAudioService _audioService;
    private readonly IOptions<QueuedLavalinkPlayerOptions> _queuePlayerOptions;
    private readonly ILogger<MusicService> _logger;
    private LavalinkTrack? _pullUpSound;

    public MusicService(
        IAudioService audioService,
        IOptions<QueuedLavalinkPlayerOptions> queuePlayerOptions
    )
    {
        _audioService = audioService;
        _queuePlayerOptions = queuePlayerOptions;
    }

    public async Task LoadPullUpSound()
    {
        var track = await _audioService.Tracks.LoadTrackAsync("7263YsyaDBc", TrackSearchMode.YouTube).ConfigureAwait(false);

        if (track is not null)
            _pullUpSound = track;
    }

    public async Task PlayTrackAsync(string query, IInteractionContext context, SearchMode? searchMode = null)
    {
        var player = await GetQueuedPlayerAsync(context, connectToVoiceChannel: true).ConfigureAwait(false);
        if (player is null)
        {
            return;
        }

        var track = await _audioService.Tracks.LoadTrackAsync(query, MapToTrackSearchMode(searchMode));
        if (track is null)
        {
            await context.Interaction.FollowupAsync("😖 No results.").ConfigureAwait(false);
            return;
        }

        var position = await player.PlayAsync(track);
        if (position is 0)
        {
            await context.Interaction.FollowupAsync($"🔈 Playing: {track.Uri}").ConfigureAwait(false);
        }
        else
        {
            await context.Interaction.FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
        }
    }

    public async Task StopAsync(IInteractionContext context)
    {
        var player = await GetQueuedPlayerAsync(context, connectToVoiceChannel: false).ConfigureAwait(false);
        if (player is null)
        {
            return;
        }

        if (player is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        await player.StopAsync().ConfigureAwait(false);

        await context.Interaction.FollowupAsync("🔇 Music stopped");
    }

    public async Task SkipAsync(IInteractionContext context)
    {
        var player = await GetQueuedPlayerAsync(context, connectToVoiceChannel: false).ConfigureAwait(false);
        if (player is null)
        {
            return;
        }

        if (player is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        await player.SkipAsync().ConfigureAwait(false);

        var newTrack = player.CurrentTrack;
        if (newTrack is not null)
        {
            await context.Interaction.FollowupAsync($"Track skipped, 🔈 Now Playing: {newTrack?.Uri}").ConfigureAwait(false);
        }
    }

    public async Task PullUpTrackAsync(IInteractionContext context)
    {
        var player = await GetQueuedPlayerAsync(context, connectToVoiceChannel: false).ConfigureAwait(false);
        if (player is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        if (_pullUpSound is null)
        {
            await context.Interaction.FollowupAsync("Pull up track has not been loaded");
            return;
        }

        var currentTrack = player.CurrentTrack;
        if (currentTrack is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        await context.Interaction.FollowupAsync("Wheel that one up");

        await PlayPercy(currentTrack, player).ConfigureAwait(false);
    }

    private async ValueTask<QueuedLavalinkPlayer?> GetQueuedPlayerAsync(IInteractionContext context, bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var user = context.User as IVoiceState;

        var result = await _audioService.Players
            .RetrieveAsync(context.Guild.Id, user?.VoiceChannel.Id, playerFactory: PlayerFactory.Queued, _queuePlayerOptions, retrieveOptions)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            await context.Interaction.FollowupAsync(errorMessage).ConfigureAwait(false);
            return null;
        }

        return result.Player;
    }

    private async Task PlayPercy(LavalinkTrack currentTrack, IQueuedLavalinkPlayer player)
    {
        // Possibly the most skuffed code anyone has ever written
        var percyTrack = await _audioService.Tracks.LoadTrackAsync("pWHY04eWimU", TrackSearchMode.YouTube);

        await player.Queue.InsertAsync(0, new TrackQueueItem(new TrackReference(currentTrack))).ConfigureAwait(false);
        await player.Queue.InsertAsync(0, new TrackQueueItem(new TrackReference(percyTrack!))).ConfigureAwait(false);

        await player.SkipAsync().ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromMilliseconds(13500)).ConfigureAwait(false);

        await player.SkipAsync().ConfigureAwait(false);
    }

    private TrackSearchMode MapToTrackSearchMode(SearchMode? searchMode)
    {
        return searchMode switch
        {
            SearchMode.YouTube => TrackSearchMode.YouTube,
            SearchMode.SoundCloud => TrackSearchMode.SoundCloud,
            _ => TrackSearchMode.YouTube,
        };
    }
}
