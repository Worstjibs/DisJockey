using Discord.WebSocket;
using DisJockey.BotService.Services.Music;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Enums;
using DisJockey.Shared.Messaging.Events;

namespace DisJockey.BotService.Consumers;

public class PlayTrackEventConsumer
{
    private readonly IMusicService _musicService;
    private readonly DiscordSocketClient _discordClient;
    private readonly IMessageSender _messageSender;

    public PlayTrackEventConsumer(
        IMusicService musicService,
        DiscordSocketClient discordClient,
        IMessageSender messageSender
    )
    {
        _musicService = musicService;
        _discordClient = discordClient;
        _messageSender = messageSender;
    }

    public async Task Consume(PlayTrackEvent playtrackEvent)
    {
        var discordUser = await _discordClient.GetUserAsync(playtrackEvent.DiscordId) as SocketUser
            ?? throw new Exception("Discord user not found");

        var guild = discordUser.MutualGuilds
            .FirstOrDefault(g => g.VoiceChannels.Any(v => v.ConnectedUsers.Any(u => u.Id == playtrackEvent.DiscordId)))
            ?? throw new Exception("User must be connected to a voice channel");

        var voiceChannel = guild.VoiceChannels.First(v => v.ConnectedUsers.Any(u => u.Id == playtrackEvent.DiscordId));

        var result = await _musicService.PlayTrackAsync(playtrackEvent.YoutubeId, guild.Id, voiceChannel.Id, playtrackEvent.Queue);

        if (!result)
        {
            return;
        }

        var trackPlayedEvent = new TrackPlayedEvent(
            playtrackEvent.YoutubeId,
            discordUser.Id,
            discordUser.GetAvatarUrl(),
            discordUser.Username,
            SearchMode.YouTube);

        await _messageSender.SendAsync(trackPlayedEvent);
    }
}
