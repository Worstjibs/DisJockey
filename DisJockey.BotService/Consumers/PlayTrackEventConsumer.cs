using Discord.WebSocket;
using DisJockey.BotService.Services.Music;
using DisJockey.Shared.Messaging.Enums;
using DisJockey.Shared.Messaging.Events;

namespace DisJockey.BotService.Consumers;

public class PlayTrackEventConsumer
{
    private readonly IMusicService _musicService;
    private readonly DiscordSocketClient _discordClient;

    public PlayTrackEventConsumer(IMusicService musicService, DiscordSocketClient discordClient)
    {
        _musicService = musicService;
        _discordClient = discordClient;
    }

    public async Task Consume(PlayTrackEvent playtrackEvent)
    {
        var discordUser = await _discordClient.GetUserAsync(playtrackEvent.DiscordId) as SocketUser
            ?? throw new Exception("Discord user not found");

        var guild = discordUser.MutualGuilds
            .FirstOrDefault(g => g.VoiceChannels.Any(v => v.ConnectedUsers.Any(u => u.Id == playtrackEvent.DiscordId)))
            ?? throw new Exception("User must be connected to a voice channel");

        var voiceChannel = guild.VoiceChannels.First(v => v.ConnectedUsers.Any(u => u.Id == playtrackEvent.DiscordId));

        await _musicService.PlayTrackAsync(
            playtrackEvent.YoutubeId,
            guild.Id,
            voiceChannel.Id,
            discordUser.Id,
            SearchMode.YouTube,
            enqueue: playtrackEvent.Queue);
    }
}
