using DisJockey.Application.Services;
using DisJockey.MassTransit.Events.BotGuilds;
using MassTransit;
using System.Threading.Tasks;

namespace DisJockey.Application.Consumers;

public class GetBotGuildsResponseEventConsumer : IConsumer<GetBotGuildsEvent.Response>
{
    private readonly BotGuildsService _botGuildsService;

    public GetBotGuildsResponseEventConsumer(BotGuildsService botGuildsService)
    {
        _botGuildsService = botGuildsService;
    }

    public async Task Consume(ConsumeContext<GetBotGuildsEvent.Response> context)
    {
        var message = context.Message;

        await _botGuildsService.AddBotGuildsAsync(message.GuidIds);
    }
}
