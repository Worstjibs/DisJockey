using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService.Services;
using DisJockey.BotService.Services.Music;
using DisJockey.BotService.Services.WheelUp;
using Lavalink4NET.Extensions;
using Lavalink4NET.Players.Queued;

namespace DisJockey.BotService;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDiscordServices(IConfiguration configuration)
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

            services.AddLavalink4NetServices(configuration);

            services.AddHostedService<HostedBotService>();

            services.AddScoped<IUserVoiceStateService, UserVoiceStateService>();

            return services;
        }

        private IServiceCollection AddLavalink4NetServices(IConfiguration configuration)
        {
            services.AddLavalink();
            services.ConfigureLavalink(config =>
            {
                config.BaseAddress = new Uri(configuration.GetConnectionString("lavalink")!);
            });

            services.Configure<QueuedLavalinkPlayerOptions>(x => new QueuedLavalinkPlayerOptions());

            services.AddScoped<IMusicService, MusicService>();
            services.AddSingleton<WheelUpService>();

            return services;
        }
    }
}
