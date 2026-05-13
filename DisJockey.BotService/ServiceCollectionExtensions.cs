using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService.Interactions;
using DisJockey.BotService.Keycloak;
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

            services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));
            services.AddHttpClient();
            services.AddSingleton<KeycloakTokenService>();

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
                config.ReadyTimeout = TimeSpan.FromMinutes(1);
            });

            services.Configure<QueuedLavalinkPlayerOptions>(x => new QueuedLavalinkPlayerOptions());

            services.AddSingleton<IMusicService, MusicService>();
            services.AddSingleton<WheelUpService>();

            return services;
        }
    }
}
