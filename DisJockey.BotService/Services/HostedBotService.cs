using Discord;
using Discord.WebSocket;
using DisJockey.BotService.Hubs;
using DisJockey.BotService.Interactions;
using DisJockey.BotService.Services.Music;
using Microsoft.Extensions.Options;

namespace DisJockey.BotService.Services;

internal class HostedBotService : BackgroundService
{
    private readonly ILogger<HostedBotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionHandler _interactionHandler;
    private readonly HubConnectionProvider _hubConnectionProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly BotSettings _settings;

    public HostedBotService(
        ILogger<HostedBotService> logger,
        DiscordSocketClient client,
        InteractionHandler interactionHandler,
        HubConnectionProvider hubConnectionProvider,
        IOptions<BotSettings> options,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _client = client;
        _interactionHandler = interactionHandler;
        _hubConnectionProvider = hubConnectionProvider;
        _serviceScopeFactory = serviceScopeFactory;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Log += LogAsync;

        await _interactionHandler.InitialiseAsync();
        await _hubConnectionProvider.InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, _settings.BotToken);
        await _client.StartAsync();

        using var scope = _serviceScopeFactory.CreateScope();

        var musicService = scope.ServiceProvider.GetRequiredService<IMusicService>();
        _client.Ready += musicService.OnReadyAsync;
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
