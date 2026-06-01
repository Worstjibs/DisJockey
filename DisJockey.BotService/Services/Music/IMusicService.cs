using DisJockey.Shared.Messaging.Enums;
using Lavalink4NET.Players.Queued;

namespace DisJockey.BotService.Services.Music;

public interface IMusicService
{
    Task<PlayTrackResult> PlayTrackAsync(string query, ulong guildId, ulong voiceChannelId, SearchMode searchMode = SearchMode.YouTube, bool enqueue = true);
    Task<string> StopAsync(ulong guildId, ulong voiceChannelId);
    Task<string> SkipAsync(ulong guildId, ulong voiceChannelId);
    Task<string> PullUpTrackAsync(ulong guildId, ulong voiceChannelId);
    Task<string> SeekAsync(ulong guildId, ulong voiceChannelId, int time);
    Task OnReadyAsync();
    ValueTask<QueuedLavalinkPlayer?> GetQueuedLavalinkPlayerAsync(ulong guildId, ulong voiceChannelId, bool connectToVoiceChannel = true);
}
