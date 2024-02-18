using Discord;
using DisJockey.MassTransit.Enums;

namespace DisJockey.BotService.Services.Music;

public interface IMusicService
{
    Task PlayTrackAsync(string query, IInteractionContext context, SearchMode searchMode = SearchMode.YouTube);
    Task StopAsync(IInteractionContext context);
    Task SkipAsync(IInteractionContext context);
    Task PullUpTrackAsync(IInteractionContext context);
    Task SeekAsync(IInteractionContext context, int time);
}