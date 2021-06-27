using API.Discord;
using API.Discord.Interfaces;
using API.Discord.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Victoria;

namespace API.Extensions {
    public static class DiscordServiceExtensions {
        public static void AddDiscordServices(this IServiceCollection services, IConfiguration config) {
            var botSettings = config.GetSection("BotSettings").Get<BotSettings>();
            services.AddSingleton<BotSettings>(botSettings);

            var client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = false
            });
            services.AddSingleton<DiscordSocketClient>(client);

            var commandService = new CommandService(new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });
            services.AddSingleton<CommandService>(commandService);

            services.AddScoped<IDiscordTrackService, DiscordTrackService>();

            services.AddLavaNode(x => {
                x.SelfDeaf = true;
                x.IsSSL = botSettings.LavalinkIsSSL;
                x.Hostname = botSettings.LavalinkHost;
                x.Port = botSettings.LavalinkPort;
                x.Authorization = botSettings.LavalinkPassword;                
            });

            services.AddSingleton<MusicService>();

            services.AddSingleton<DiscordService>();
            // services.AddHostedService<DiscordService>(provider => provider.GetService<DiscordService>());
        }
    }
}