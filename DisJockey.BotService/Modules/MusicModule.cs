using Discord.Interactions;
using DisJockey.BotService.Services.Music;
using DisJockey.Shared.Messaging.Enums;

namespace DisJockey.BotService.Modules;

public class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<MusicModule> _logger;
    private readonly IMusicService _musicService;

    public MusicModule(
        ILogger<MusicModule> logger,
        IMusicService musicService)
    {
        _logger = logger;
        _musicService = musicService;
    }

    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query, [MinLength(0)] SearchMode searchMode = SearchMode.YouTube) =>
        await DeferActionAsync(async () => await _musicService.PlayTrackAsync(query, Context, searchMode));

    [SlashCommand("stop", description: "Stops music", runMode: RunMode.Async)]
    public async Task Stop() => await DeferActionAsync(async () => await _musicService.StopAsync(Context).ConfigureAwait(false));

    [SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
    public async Task Skip() => await DeferActionAsync(async () => await _musicService.SkipAsync(Context).ConfigureAwait(false));

    [SlashCommand("pull-it", description: "If it's nice, play it twice", runMode: RunMode.Async)]
    public async Task PullIt() => await DeferActionAsync(async () => await _musicService.PullUpTrackAsync(Context).ConfigureAwait(false));

    [SlashCommand("seek", description: "Seek a track to a given time in seconds", runMode: RunMode.Async)]
    public async Task Seek(int time) => await DeferActionAsync(async () => await _musicService.SeekAsync(Context, time).ConfigureAwait(false));

    private async Task DeferActionAsync(Func<Task> deferredAction)
    {
        try
        {
            await DeferAsync().ConfigureAwait(false);
            await deferredAction().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured processing the action.");

            await Context.Interaction.FollowupAsync("Something went wrong");
        }
    }
}
