using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using API.DTOs;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace API.Discord {
    public class DiscordBot : BackgroundService {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public DiscordBot(IConfiguration config, HttpClient httpClient) {
            _httpClient = httpClient;
            _config = config;

            _client = new DiscordSocketClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await MainAsync();
        }

        public async Task MainAsync() {
            _client.MessageReceived += MessageReceivedAsync;

            await _client.LoginAsync(TokenType.Bot, _config.GetValue<string>("Discord:BotToken"));
            await _client.StartAsync();
        }

        private async Task MessageReceivedAsync(SocketMessage message) {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content.StartsWith("-playTest")) {
                long discordId = (long)message.Author.Id;
                string[] commands = message.Content.Split(" ");

                string url = commands[1];

                if (url.Contains("youtu.be") || url.Contains("youtube.com")) {
                    await AddTrackAsync(discordId, url);
                }
                // await message.Channel.SendMessageAsync("Track Played");
            }
        }

        private async Task AddTrackAsync(long discordId, string url) {
            var trackAddDto = new TrackAddDto {
                DiscordId = discordId,
                URL = url
            };

            var dtoToStr = JsonSerializer.Serialize(trackAddDto);
            var dtoStrContent = new StringContent(dtoToStr, Encoding.UTF8, "application/json");

            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            var response = await _httpClient.PostAsync("/api/track", dtoStrContent);
        }
    }
}