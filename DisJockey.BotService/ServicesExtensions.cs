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

        services.AddSingleton<InteractionService>();
        services.AddSingleton<InteractionHandler>();

        services.AddLavalink();
        services.ConfigureLavalink(config =>
        {
            config.BaseAddress = new Uri("http://lavalink:2333");
        });

        services.Configure<QueuedLavalinkPlayerOptions>(x => new QueuedLavalinkPlayerOptions());

        services.AddSingleton<IMusicService, MusicService>();
        services.AddSingleton<WheelUpService>();

        services.AddHostedService<HostedBotService>();

        return services;
    }
}
