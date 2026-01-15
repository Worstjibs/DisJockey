using DisJockey.Application.Services;
using DisJockey.Shared.Messaging.Events.BotGuilds;

namespace DisJockey.Application.Consumers;

public class GetBotGuildsResponseEventConsumer
{
    private readonly BotGuildsService _botGuildsService;

    public GetBotGuildsResponseEventConsumer(BotGuildsService botGuildsService)
    {
        _botGuildsService = botGuildsService;
    }

    public async Task Consume(GetBotGuildsEvent.Response responseMessage)
    {
        await _botGuildsService.AddBotGuildsAsync(responseMessage.GuildIds);
    }
}
