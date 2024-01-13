using Discord;
using Lavalink4NET;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Microsoft.Extensions.Options;
using DisJockey.BotService.Services.WheelUp;
using MassTransit;
using DisJockey.Shared.Enums;
using DisJockey.Shared.Events;
using Discord.WebSocket;

namespace DisJockey.BotService.Services.Music;

public class MusicService : IMusicService
{
    private readonly IAudioService _audioService;
    private readonly IOptions<QueuedLavalinkPlayerOptions> _queuePlayerOptions;
    private readonly WheelUpService _wheelUpService;
    private readonly IBus _bus;

    public MusicService(
        IAudioService audioService,
        IOptions<QueuedLavalinkPlayerOptions> queuePlayerOptions,
        WheelUpService wheelUpService,
        IBus bus
    )
    {
        _audioService = audioService;
        _queuePlayerOptions = queuePlayerOptions;
        _wheelUpService = wheelUpService;
        _bus = bus;
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

        await _bus.Publish(new TrackPlayedEvent
        {
            SearchMode = searchMode,
            TrackId = track.Identifier,
            DiscordId = socketUser.Id,
            AvatarUrl = socketUser.GetAvatarUrl(),
            UserName = socketUser.Username
        });

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
