using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using API.Discord.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API.Discord {
    public class DiscordBot : BackgroundService {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BotSettings _botSettings;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public DiscordBot(
            IConfiguration config,
            HttpClient httpClient,
            IServiceScopeFactory serviceScopeFactory,
            DiscordSocketClient client,
            BotSettings botSettings,
            CommandService cmdService,
            IServiceProvider services
        ) {

            _cmdService = cmdService;
            _services = services;
            _client = client;
            _botSettings = botSettings;
            _serviceScopeFactory = serviceScopeFactory;
            _httpClient = httpClient;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await MainAsync();
        }

        public async Task MainAsync() {
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;

            await _client.LoginAsync(TokenType.Bot, _botSettings.BotToken);
            await _client.StartAsync();

            var cmdHandler = new CommandHandler(_client, _cmdService, _services);
            await cmdHandler.InitializeAsync();
        }

        private async Task JoinChannel(SocketMessage message) {
            var user = message.Author as SocketGuildUser;
            var voiceChannel = user.VoiceChannel;

            if (voiceChannel == null)
                await message.Channel.SendMessageAsync("You must be connected to a voice channel");

            await voiceChannel.ConnectAsync();
        }

        public async Task<bool> PlayTrack(string youtubeId) {
            var guilds = _client.Guilds;
            var nascar = (ITextChannel)_client.GetChannel(706809072867082340);

            await nascar.SendMessageAsync($"-play https://youtu.be/{youtubeId}");

            return true;
        }

        private async Task AddTrackAsync(long discordId, string url) {
            using (var scope = _serviceScopeFactory.CreateScope()) {
                var discordTrackService = scope.ServiceProvider.GetService<IDiscordTrackService>();

                await discordTrackService.AddTrackAsync(discordId, url);
            }
        }
        private Task LogAsync(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync() {
            Console.WriteLine($"Connected as -> {_client.CurrentUser.Username}");
            return Task.CompletedTask;
        }
    }
}