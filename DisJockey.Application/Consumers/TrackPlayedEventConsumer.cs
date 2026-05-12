using DisJockey.Application.Interfaces;
using DisJockey.Shared.Messaging.Events;
using static DisJockey.Application.Interfaces.IDiscordTrackService;

namespace DisJockey.Application.Consumers;

public class TrackPlayedEventConsumer
{
    private readonly IDiscordTrackService _discordTrackService;

    public TrackPlayedEventConsumer(IDiscordTrackService discordTrackService)
    {
        _discordTrackService = discordTrackService;
    }

    public async Task Consume(TrackPlayedEvent trackPlayedEvent)
    {
        var addTrackArgs = new AddTrackArgs
        {
            Username = trackPlayedEvent.UserName,
            AvatarUrl = trackPlayedEvent.AvatarUrl,
            DiscordId = trackPlayedEvent.DiscordId
        };

        await _discordTrackService.AddTrackAsync(addTrackArgs, trackPlayedEvent.TrackId);
    }
}
