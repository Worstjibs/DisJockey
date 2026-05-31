using DisJockey.Shared.Messaging.Enums;
using Lavalink4NET.Players.Queued;

namespace DisJockey.BotService.Services.Music;

public interface IMusicService
{
    Task<string?> PlayTrackAsync(string query, ulong guildId, ulong voiceChannelId, ulong userId, string avatarUrl, string username, SearchMode searchMode = SearchMode.YouTube);
    Task<bool> PlayTrackAsync(string youtubeId, ulong guildId, ulong voiceChannelId, bool queue);
    Task<string> StopAsync(ulong guildId, ulong voiceChannelId);
    Task<string> SkipAsync(ulong guildId, ulong voiceChannelId);
    Task<string> PullUpTrackAsync(ulong guildId, ulong voiceChannelId);
    Task<string> SeekAsync(ulong guildId, ulong voiceChannelId, int time);
    Task OnReadyAsync();
    ValueTask<QueuedLavalinkPlayer?> GetQueuedLavalinkPlayerAsync(ulong guildId, ulong voiceChannelId, bool connectToVoiceChannel = true);
}
