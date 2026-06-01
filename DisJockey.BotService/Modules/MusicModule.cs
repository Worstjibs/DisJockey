using Discord;
using Discord.Interactions;
using DisJockey.BotService.Services.Music;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Enums;
using DisJockey.Shared.Messaging.Events;

namespace DisJockey.BotService.Modules;

public class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<MusicModule> _logger;
    private readonly IMusicService _musicService;
    private readonly IMessageSender _messageSender;

    public MusicModule(
        ILogger<MusicModule> logger,
        IMusicService musicService,
        IMessageSender messageSender)
    {
        _logger = logger;
        _musicService = musicService;
        _messageSender = messageSender;
    }

    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query, [MinLength(0)] SearchMode searchMode = SearchMode.YouTube)
    {
        await DeferActionAsync(async () =>
        {
            var (guildId, voiceChannelId) = GetGuildAndVoiceChannel();
            if (voiceChannelId is null)
            {
                return "You are not connected to a voice channel.";
            }

            var result = await _musicService.PlayTrackAsync(query, guildId, voiceChannelId.Value, searchMode);

            if (result.IsSuccess && searchMode is SearchMode.YouTube)
            {
                var trackPlayedEvent = new TrackPlayedEvent(
                    result.TrackIdentifier!,
                    Context.User.Id,
                    Context.User.GetAvatarUrl(),
                    Context.User.Username,
                    SearchMode.YouTube);

                await _messageSender.SendAsync(trackPlayedEvent);
            }

            return result.Message;
        });
    }

    [SlashCommand("stop", description: "Stops music", runMode: RunMode.Async)]
    public async Task Stop()
    {
        await DeferActionAsync(async () =>
        {
            var (guildId, voiceChannelId) = GetGuildAndVoiceChannel();
            if (voiceChannelId is null)
            {
                return "You are not connected to a voice channel.";
            }

            return await _musicService.StopAsync(guildId, voiceChannelId.Value);
        });
    }

    [SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
    public async Task Skip()
    {
        await DeferActionAsync(async () =>
        {
            var (guildId, voiceChannelId) = GetGuildAndVoiceChannel();
            if (voiceChannelId is null)
            {
                return "You are not connected to a voice channel.";
            }

            return await _musicService.SkipAsync(guildId, voiceChannelId.Value);
        });
    }

    [SlashCommand("pull-it", description: "If it's nice, play it twice", runMode: RunMode.Async)]
    public async Task PullIt()
    {
        await DeferActionAsync(async () =>
        {
            var (guildId, voiceChannelId) = GetGuildAndVoiceChannel();
            if (voiceChannelId is null)
            {
                return "You are not connected to a voice channel.";
            }

            return await _musicService.PullUpTrackAsync(guildId, voiceChannelId.Value);
        });
    }

    [SlashCommand("seek", description: "Seek a track to a given time in seconds", runMode: RunMode.Async)]
    public async Task Seek(int time)
    {
        await DeferActionAsync(async () =>
        {
            var (guildId, voiceChannelId) = GetGuildAndVoiceChannel();
            if (voiceChannelId is null)
            {
                return "You are not connected to a voice channel.";
            }

            return await _musicService.SeekAsync(guildId, voiceChannelId.Value, time);
        });
    }

    private (ulong GuildId, ulong? VoiceChannelId) GetGuildAndVoiceChannel()
    {
        var voiceChannelId = (Context.User as IVoiceState)?.VoiceChannel?.Id;
        return (Context.Guild.Id, voiceChannelId);
    }

    private async Task DeferActionAsync(Func<Task<string?>> deferredAction)
    {
        try
        {
            await DeferAsync().ConfigureAwait(false);
            var response = await deferredAction().ConfigureAwait(false);
            if (response is not null)
            {
                await FollowupAsync(response).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured processing the action.");
            await Context.Interaction.FollowupAsync("Something went wrong");
        }
    }
}
