using Discord;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using DisJockey.BotService.Services.WheelUp;
using Discord.WebSocket;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Tracks;
using DisJockey.Shared.Messaging.Events;
using DisJockey.Shared.Messaging.Enums;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.BotService.Players;

namespace DisJockey.BotService.Services.Music;

public class MusicService : IMusicService
{
    private readonly IAudioService _audioService;
    private readonly IOptions<QueuedLavalinkPlayerOptions> _queuePlayerOptions;
    private readonly WheelUpService _wheelUpService;
    private readonly IMessageSender _messageSender;

    public MusicService(
        IAudioService audioService,
        IOptions<QueuedLavalinkPlayerOptions> queuePlayerOptions,
        WheelUpService wheelUpService,
        IMessageSender messageSender
    )
    {
        _audioService = audioService;
        _queuePlayerOptions = queuePlayerOptions;
        _wheelUpService = wheelUpService;
        _messageSender = messageSender;
    }

    public async Task PlayTrackAsync(string query, IInteractionContext context, SearchMode searchMode = SearchMode.YouTube)
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

        var socketUser = (context.User as SocketUser)!;

        if (track.Provider is StreamProvider.YouTube)
        {
            var trackPlayedEvent = new TrackPlayedEvent(
                                            track.Identifier,
                                            socketUser.Id,
                                            socketUser.GetAvatarUrl(),
                                            socketUser.Username,
                                            SearchMode.YouTube);

            await _messageSender.SendAsync(trackPlayedEvent);
        }

        if (position is 0)
        {
            await context.Interaction.FollowupAsync($"🔈 Playing: {track.Uri}").ConfigureAwait(false);
        }
        else
        {
            await context.Interaction.FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
        }
    }

    public async Task<bool> PlayTrackAsync(string youtubeId, SocketUser discordUser, SocketGuild guild, bool queue)
    {
        var voiceChannel = guild.VoiceChannels.First(x => x.ConnectedUsers.Any(u => u.Id == discordUser.Id));

        var retrieveOptions = new PlayerRetrieveOptions(PlayerChannelBehavior.Join);

        var playerResult = await _audioService.Players.RetrieveAsync(guild.Id, voiceChannel.Id, playerFactory: PlayerFactory.Queued, _queuePlayerOptions, retrieveOptions);
        if (!playerResult.IsSuccess)
            return false;

        var track = await _audioService.Tracks.LoadTrackAsync(youtubeId, TrackSearchMode.YouTube);
        if (track is null)
            return false;

        await playerResult.Player.PlayAsync(track, enqueue: queue);

        return true;
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
        else
        {
            await context.Interaction.FollowupAsync($"Nothing left in the queue, disconnecting").ConfigureAwait(false);
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

        var currentTrack = player.CurrentTrack;
        if (currentTrack is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        await context.Interaction.FollowupAsync("Wheel that one up");

        await _wheelUpService.PullUp(currentTrack, player).ConfigureAwait(false);
    }

    public async Task SeekAsync(IInteractionContext context, int time)
    {
        var player = await GetQueuedPlayerAsync(context, connectToVoiceChannel: false).ConfigureAwait(false);
        if (player is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        var currentTrack = player.CurrentTrack;
        if (currentTrack is null)
        {
            await context.Interaction.FollowupAsync("Player is not currently playing");
            return;
        }

        await player.SeekAsync(TimeSpan.FromSeconds(time));

        await context.Interaction.FollowupAsync($"Track seeked to {time} seconds");
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

    private async ValueTask<QueuedLavalinkPlayer?> GetQueuedPlayerAsync(IInteractionContext context, bool connectToVoiceChannel = true)
    {
        var guildId = context.Guild.Id;

        var user = context.User as IVoiceState;

        var playerResult = await GetQueuedPlayerResultAsync(guildId, user!.VoiceChannel.Id, connectToVoiceChannel).ConfigureAwait(false);
        if (!playerResult.IsSuccess)
        {
            var errorMessage = playerResult.Status switch
            {
                PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
                _ => "Unknown error.",
            };

            await context.Interaction.FollowupAsync(errorMessage).ConfigureAwait(false);
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
                await queuedPlayer.DisconnectAsync();
        }

        return Task.CompletedTask;
    }
}
