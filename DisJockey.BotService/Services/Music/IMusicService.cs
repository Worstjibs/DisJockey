using Discord;
using DisJockey.BotService.Modules;

namespace DisJockey.BotService.Services.Music;

public interface IMusicService
{
    Task PlayTrackAsync(string query, IInteractionContext context, SearchMode? searchMode = null);
    Task StopAsync(IInteractionContext context);
    Task SkipAsync(IInteractionContext context);
    Task PullUpTrackAsync(IInteractionContext context);
    Task SeekAsync(IInteractionContext context, int time);
}