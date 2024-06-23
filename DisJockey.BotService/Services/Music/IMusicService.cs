using Discord;
using Discord.WebSocket;
using DisJockey.MassTransit.Enums;

namespace DisJockey.BotService.Services.Music;

public interface IMusicService
{
    Task PlayTrackAsync(string query, IInteractionContext context, SearchMode searchMode = SearchMode.YouTube);
    Task<bool> PlayTrackAsync(string youtubeId, SocketUser discordUser, SocketGuild guild, bool queue);
    Task StopAsync(IInteractionContext context);
    Task SkipAsync(IInteractionContext context);
    Task PullUpTrackAsync(IInteractionContext context);
    Task SeekAsync(IInteractionContext context, int time);
    Task OnReadyAsync();
}