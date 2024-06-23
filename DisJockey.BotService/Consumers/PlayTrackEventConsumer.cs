using Discord.WebSocket;
using DisJockey.BotService.Services.Music;
using DisJockey.MassTransit.Enums;
using DisJockey.MassTransit.Events;
using MassTransit;

namespace DisJockey.BotService.Consumers;

public class PlayTrackEventConsumer : IConsumer<PlayTrackEvent>
{
    private readonly IMusicService _musicService;
    private readonly DiscordSocketClient _discordClient;
    private readonly IBus _bus;

    public PlayTrackEventConsumer(
        IMusicService musicService,
        DiscordSocketClient discordClient,
        IBus bus
    )
    {
        _musicService = musicService;
        _discordClient = discordClient;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<PlayTrackEvent> context)
    {
        var message = context.Message;

        var discordUser = await _discordClient.GetUserAsync(message.DiscordId) as SocketUser;
        if (discordUser is null)
            throw new Exception("Discord user not found");

        var guild = discordUser.MutualGuilds.FirstOrDefault(g => g.VoiceChannels.Any(v => v.ConnectedUsers.Any(u => u.Id == message.DiscordId)));
        if (guild is null)
            throw new Exception("Music first be connected to a voice channel");

        var result = await _musicService.PlayTrackAsync(message.YoutubeId, discordUser, guild, message.Queue);

        if (!result)
            return;

        await _bus.Publish(new TrackPlayedEvent
        {
            SearchMode = SearchMode.YouTube,
            TrackId = message.YoutubeId,
            DiscordId = discordUser.Id,
            AvatarUrl = discordUser.GetAvatarUrl(),
            UserName = discordUser.Username
        });
    }
}
