using System;
using System.Threading;
using System.Threading.Tasks;
using API.Discord.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Victoria;

namespace API.Discord {
    public class DiscordBot : BackgroundService {
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BotSettings _botSettings;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;
        private readonly LavaNode _lavaNode;

        public DiscordBot(
            IConfiguration config, 
            IServiceScopeFactory serviceScopeFactory,
            DiscordSocketClient client,
            BotSettings botSettings,
            CommandService cmdService,
            IServiceProvider services,
            LavaNode lavaNode
        ) {

            _cmdService = cmdService;
            _services = services;
            _client = client;
            _botSettings = botSettings;
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
            _lavaNode = lavaNode;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await MainAsync();
        }

        public async Task MainAsync() {
            _client.Log += LogAsync;

            await _client.LoginAsync(TokenType.Bot, _botSettings.BotToken);
            await _client.StartAsync();

            var cmdHandler = new CommandHandler(_client, _cmdService, _services, _botSettings);
            await cmdHandler.InitializeAsync();

            _client.Ready += ReadyAsync;
        }

        private Task LogAsync(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyAsync() {
            if (!_lavaNode.IsConnected) {
                _lavaNode.OnLog += LogAsync;
                await _lavaNode.ConnectAsync();
            }

            Console.WriteLine($"Connected as -> {_client.CurrentUser.Username}");
        }
    }
}