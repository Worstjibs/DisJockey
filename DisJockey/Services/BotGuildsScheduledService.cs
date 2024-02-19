using DisJockey.MassTransit.Events.BotGuilds;
using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DisJockey.Services;

public class BotGuildsScheduledService : BackgroundService
{
    private readonly IBus _bus;

    public BotGuildsScheduledService(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the bot to login
        await Task.Delay(10000);

        await _bus.Publish(new GetBotGuildsEvent(), stoppingToken);
    }
}
