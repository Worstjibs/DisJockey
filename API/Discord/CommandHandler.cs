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
        private readonly BotSettings _botSettings;
        public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider services, BotSettings botSettings) {
            _botSettings = botSettings;
            _services = services;
            _commandService = commandService;
            _client = client;
        }

        public async Task InitializeAsync() {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _commandService.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        private async Task MessageReceivedAsync(SocketMessage message) {
            var argPos = 0;

            if (message.Author.IsBot)
                return;

            var userMessage = message as SocketUserMessage;
            if (userMessage is null)
                return;

            if (!userMessage.HasCharPrefix(_botSettings.Prefix, ref argPos))
                return;

            var context = new SocketCommandContext(_client, userMessage);
            var result = await _commandService.ExecuteAsync(context, argPos, _services);
        }

        private Task LogAsync(LogMessage logMessage) {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}