using DisJockey.MassTransit.Events;
using DisJockey.Services.Interfaces;
using MassTransit;
using System.Threading.Tasks;
using static DisJockey.Services.Interfaces.IDiscordTrackService;

namespace DisJockey.Consumers;

public class TrackPlayedEventConsumer : IConsumer<TrackPlayedEvent>
{
    private readonly IDiscordTrackService _discordTrackService;

    public TrackPlayedEventConsumer(IDiscordTrackService discordTrackService)
    {
        _discordTrackService = discordTrackService;
    }

    public async Task Consume(ConsumeContext<TrackPlayedEvent> context)
    {
        var message = context.Message;

        var addTrackArgs = new AddTrackArgs
        {
            Username = message.UserName,
            AvatarUrl = message.AvatarUrl,
            DiscordId = message.DiscordId
        };

        await _discordTrackService.AddTrackAsync(addTrackArgs, message.TrackId);
    }
}
