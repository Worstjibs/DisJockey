using Discord;
using Discord.WebSocket;
using DisJockey.MassTransit.Events.BotGuilds;
using MassTransit;

namespace DisJockey.BotService.Consumers;

public class GetBotGuildsEventConsumer : IConsumer<GetBotGuildsEvent>
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IBus _bus;

    public GetBotGuildsEventConsumer(DiscordSocketClient discordSocketClient, IBus bus)
    {
        _discordSocketClient = discordSocketClient;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<GetBotGuildsEvent> context)
    {
        if (_discordSocketClient.LoginState != LoginState.LoggedIn)
            return;

        var botGuilds = _discordSocketClient.Guilds.Select(x => x.Id).ToArray();

        var response = new GetBotGuildsEvent.Response { GuidIds = botGuilds };

        await _bus.Publish(response, context.CancellationToken);
    }
}
