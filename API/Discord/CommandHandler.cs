using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace API.Discord {
    public class CommandHandler {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;
        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services) {
            _services = services;
            _commandService = commandService;
            _client = client;
        }

        public async Task InitializeAsync() {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            
            _client.MessageReceived += MessageReceivedAsync;
        }

        private async Task MessageReceivedAsync(SocketMessage message) {
            if (message.Author.IsBot)
                return;

            var userMessage = message as SocketUserMessage;

            // if (message.Content.StartsWith("-playTest")) {
            //     long discordId = (long)message.Author.Id;
            //     string[] commands = message.Content.Split(" ");

            //     string url = commands[1];

            //     if (url.Contains("youtu.be") || url.Contains("youtube.com")) {
            //         // await AddTrackAsync(discordId, url);
            //     }
            //     await message.Channel.SendMessageAsync("Track Played");
            // }

            if (message.Content.StartsWith("-hello")) {
                await message.Channel.SendMessageAsync("Hello from Command Handler");
            }
        }

        private Task LogAsync(LogMessage logMessage) {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}