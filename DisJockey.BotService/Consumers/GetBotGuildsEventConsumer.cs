using Discord;
using Discord.WebSocket;
using DisJockey.Shared.Messaging.Events.BotGuilds;

namespace DisJockey.BotService.Consumers;

public class GetBotGuildsEventConsumer
{
    private readonly DiscordSocketClient _discordSocketClient;

    public GetBotGuildsEventConsumer(DiscordSocketClient discordSocketClient)
    {
        _discordSocketClient = discordSocketClient;
    }

    public async Task Consume(GetBotGuildsEvent @event)
    {
        if (_discordSocketClient.LoginState != LoginState.LoggedIn)
            return;

        var botGuilds = _discordSocketClient.Guilds.Select(x => x.Id).ToArray();

        var response = new GetBotGuildsEvent.Response { GuildIds = botGuilds };

        // TODO: Send message here
    }
}
