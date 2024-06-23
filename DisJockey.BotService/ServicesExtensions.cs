using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService.Services.Music;
using DisJockey.BotService.Services.WheelUp;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;

namespace DisJockey.BotService;

public static class ServicesExtensions
{
    public static IServiceCollection AddDiscordServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BotSettings>(configuration.GetSection("BotSettings"));

        services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            LogLevel = LogSeverity.Debug,
            GatewayIntents = GatewayIntents.All,
            UseInteractionSnowflakeDate = true
        }));

        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<InteractionHandler>();

        services.AddLavalink4NetServices();

        services.AddHostedService<HostedBotService>();

        return services;
    }

    private static IServiceCollection AddLavalink4NetServices(this IServiceCollection services)
    {
        services.AddLavalink();
        services.ConfigureLavalink(config =>
        {
            config.BaseAddress = new Uri("http://lavalink:2333");
        });

        services.Configure<QueuedLavalinkPlayerOptions>(x => new QueuedLavalinkPlayerOptions());

        services.AddSingleton<IMusicService, MusicService>();
        services.AddSingleton<WheelUpService>();

        return services;
    }
}
