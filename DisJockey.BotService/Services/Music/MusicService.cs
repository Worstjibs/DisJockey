using DisJockey.BotService.Players;
using DisJockey.BotService.Services.Events;
using DisJockey.BotService.Services.WheelUp;
using DisJockey.Shared.Messaging.Enums;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.Tracks;
using Microsoft.Extensions.Options;

namespace DisJockey.BotService.Services.Music;

public class MusicService : IMusicService
{
    private readonly IAudioService _audioService;
    private readonly IOptions<QueuedLavalinkPlayerOptions> _queuePlayerOptions;
    private readonly WheelUpService _wheelUpService;
    private readonly IEventsPublisher _trackPlayedPublisher;

    public MusicService(
        IAudioService audioService,
        IOptions<QueuedLavalinkPlayerOptions> queuePlayerOptions,
        WheelUpService wheelUpService,
        IEventsPublisher trackPlayedPublisher
    )
    {
        _audioService = audioService;
        _queuePlayerOptions = queuePlayerOptions;
        _wheelUpService = wheelUpService;
        _trackPlayedPublisher = trackPlayedPublisher;
    }

    public async Task<string> PlayTrackAsync(
        string query,
        ulong guildId,
        ulong voiceChannelId,
        ulong userId,
        SearchMode searchMode = SearchMode.YouTube,
        bool enqueue = true)
    {
        var playerResult = await GetQueuedPlayerResultAsync(guildId, voiceChannelId, connectToVoiceChannel: true).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            return GetPlayerErrorMessage(playerResult.Status);
        }

        var sanitizedQuery = RegexHelpers.StripSpecialCharacters(query);

        var track = await _audioService.Tracks.LoadTrackAsync(sanitizedQuery, MapToTrackSearchMode(searchMode));
        if (track is null)
        {
            return "😖 No results.";
        }

        var position = await playerResult.Player.PlayAsync(track, enqueue: enqueue);

        if (track.Provider is StreamProvider.YouTube)
        {
            await _trackPlayedPublisher.PublishTrackPlayedAsync(userId, track.Identifier, searchMode);
        }

        return position is 0
            ? $"🔈 Playing: {track.Uri}"
            : $"🔈 Added to queue: {track.Uri}";
    }

    public async Task<string> StopAsync(ulong guildId, ulong voiceChannelId)
    {
        var playerResult = await GetQueuedPlayerResultAsync(guildId, voiceChannelId, connectToVoiceChannel: false).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            return GetPlayerErrorMessage(playerResult.Status);
        }

        await playerResult.Player.StopAsync().ConfigureAwait(false);

        return "🔇 Music stopped";
    }

    public async Task<string> SkipAsync(ulong guildId, ulong voiceChannelId)
    {
        var playerResult = await GetQueuedPlayerResultAsync(guildId, voiceChannelId, connectToVoiceChannel: false).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            return GetPlayerErrorMessage(playerResult.Status);
        }

        await playerResult.Player.SkipAsync().ConfigureAwait(false);

        var newTrack = playerResult.Player.CurrentTrack;
        if (newTrack is not null)
        {
            return $"Track skipped, 🔈 Now Playing: {newTrack.Uri}";
        }

        return "Nothing left in the queue, disconnecting";
    }

    public async Task<string> PullUpTrackAsync(ulong guildId, ulong voiceChannelId)
    {
        var playerResult = await GetQueuedPlayerResultAsync(guildId, voiceChannelId, connectToVoiceChannel: false).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            return GetPlayerErrorMessage(playerResult.Status);
        }

        var currentTrack = playerResult.Player.CurrentTrack;
        if (currentTrack is null)
        {
            return "Player is not currently playing";
        }

        await _wheelUpService.PullUp(currentTrack, playerResult.Player).ConfigureAwait(false);

        return "Wheel that one up";
    }

    public async Task<string> SeekAsync(ulong guildId, ulong voiceChannelId, int time)
    {
        var playerResult = await GetQueuedPlayerResultAsync(guildId, voiceChannelId, connectToVoiceChannel: false).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            return GetPlayerErrorMessage(playerResult.Status);
        }

        if (playerResult.Player.CurrentTrack is null)
        {
            return "Player is not currently playing";
        }

        await playerResult.Player.SeekAsync(TimeSpan.FromSeconds(time));

        return $"Track seeked to {time} seconds";
    }

    public async ValueTask<QueuedLavalinkPlayer?> GetQueuedLavalinkPlayerAsync(
        ulong guildId,
        ulong voiceChannelId,
        bool connectToVoiceChannel = true)
    {
        var playerResult = await GetQueuedPlayerResultAsync(guildId, voiceChannelId, connectToVoiceChannel).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            return null;
        }


        return playerResult.Player;
    }

    private async ValueTask<PlayerResult<NotifyingPlayer>> GetQueuedPlayerResultAsync(
        ulong guildId,
        ulong voiceChannelId,
        bool connectToVoiceChannel = true)
    {
        var retrieveOptions = new PlayerRetrieveOptions(
            ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

        var result = await _audioService.Players
                                .RetrieveAsync<NotifyingPlayer, QueuedLavalinkPlayerOptions>(
                                    guildId,
                                    voiceChannelId,
                                    playerFactory: NotifyingPlayer.CreatePlayerAsync,
                                    _queuePlayerOptions,
                                    retrieveOptions)
                                .ConfigureAwait(false);

        return result;
    }

    private static string GetPlayerErrorMessage(PlayerRetrieveStatus status) => status switch
    {
        PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
        PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
        _ => "Unknown error.",
    };

    private static TrackSearchMode MapToTrackSearchMode(SearchMode? searchMode)
    {
        return searchMode switch
        {
            SearchMode.YouTube => TrackSearchMode.YouTube,
            SearchMode.SoundCloud => TrackSearchMode.SoundCloud,
            _ => TrackSearchMode.YouTube,
        };
    }

    public Task OnReadyAsync()
    {
        _audioService.TrackEnded += OnTrackEnded;

        async Task OnTrackEnded(object sender, TrackEndedEventArgs eventArgs)
        {
            var queuedPlayer = eventArgs.Player as IQueuedLavalinkPlayer;

            if (queuedPlayer is not null && queuedPlayer.State == PlayerState.NotPlaying)
            {
                await queuedPlayer.DisconnectAsync();
            }
        }

        return Task.CompletedTask;
    }
}
