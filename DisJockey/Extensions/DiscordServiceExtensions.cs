using DisJockey.Discord;
using DisJockey.Discord.Services;
using DisJockey.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using DisJockey.Services.Interfaces;
using Discord.Rest;

namespace DisJockey.Extensions
{
    public static class DiscordServiceExtensions
    {
        public static void AddDiscordServices(this IServiceCollection services, IConfiguration config)
        {
            var botSettings = config.GetSection("BotSettings").Get<BotSettings>();
            services.AddSingleton(botSettings);

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = botSettings.LogLevel,
                AlwaysDownloadUsers = false,
                GatewayIntents = GatewayIntents.All
            });
            services.AddSingleton(client);

            var commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = botSettings.LogLevel,
                CaseSensitiveCommands = false
            });
            services.AddSingleton(commandService);

            services.AddScoped<IDiscordTrackService, DiscordTrackService>();

            services.AddLavaNode(x =>
            {
                x.SelfDeaf = true;
                x.IsSsl = botSettings.LavalinkIsSSL;
                x.Hostname = botSettings.LavalinkHost;
                x.Port = botSettings.LavalinkPort;
                x.Authorization = botSettings.LavalinkPassword;
                x.LogSeverity = botSettings.LogLevel;
            });

            services.AddSingleton<MusicService>();

            services.AddSingleton<DiscordService>();
            services.AddHostedService(provider => provider.GetService<DiscordService>());
        }
    }
}