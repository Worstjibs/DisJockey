using API.Discord;
using API.Discord.Interfaces;
using API.Discord.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions {
    public static class DiscordServiceExtensions {
        public static IServiceCollection AddDiscordServices(this IServiceCollection services, IConfiguration config) {
            var botSettings = config.GetSection("BotSettings").Get<BotSettings>();

            var client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = false
            });

            var commandService = new CommandService(new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });
            
            services.AddSingleton<DiscordSocketClient>(client);
            services.AddSingleton<BotSettings>(botSettings);
            services.AddSingleton<CommandService>(commandService);

            services.AddScoped<IDiscordTrackService, DiscordTrackService>();

            services.AddSingleton<DiscordBot>();
            services.AddHostedService<DiscordBot>(provider => provider.GetService<DiscordBot>());

            return services;
        }
    }
}