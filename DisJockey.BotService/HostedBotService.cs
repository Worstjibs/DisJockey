using Discord;
using Discord.WebSocket;
using DisJockey.BotService.Services;
using Microsoft.Extensions.Options;

namespace DisJockey.BotService;

internal class HostedBotService : BackgroundService
{
    private readonly ILogger<HostedBotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionHandler _interactionHandler;
    private readonly IMusicService _musicService;
    private readonly BotSettings _settings;

    public HostedBotService(
        ILogger<HostedBotService> logger,
        DiscordSocketClient client,
        InteractionHandler interactionHandler,
        IMusicService musicService,
        IOptions<BotSettings> options
    )
    {
        _logger = logger;
        _client = client;
        _interactionHandler = interactionHandler;
        _musicService = musicService;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Log += LogAsync;

        await _interactionHandler.InitialiseAsync();

        await _musicService.LoadPullUpSound();

        await _client.LoginAsync(TokenType.Bot, _settings.BotToken);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        _logger.Log(severity, "[{Source}] {Message}", message.Source, message.Message);

        return Task.CompletedTask;
    }
}
