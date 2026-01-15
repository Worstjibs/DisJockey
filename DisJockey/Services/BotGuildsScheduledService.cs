using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Events.BotGuilds;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DisJockey.Services;

public class BotGuildsScheduledService : BackgroundService
{
    private readonly IMessageSender _messageSender;

    public BotGuildsScheduledService(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the bot to login
        await Task.Delay(10000, stoppingToken);

        await _messageSender.SendAsync(new GetBotGuildsEvent());
    }
}
