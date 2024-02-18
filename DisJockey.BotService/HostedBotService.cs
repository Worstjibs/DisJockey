using Discord;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Microsoft.Extensions.Options;

namespace DisJockey.BotService;

internal class HostedBotService : BackgroundService
{
    private readonly ILogger<HostedBotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionHandler _interactionHandler;
    private readonly IAudioService _audioService;
    private readonly BotSettings _settings;

    public HostedBotService(
        ILogger<HostedBotService> logger,
        DiscordSocketClient client,
        InteractionHandler interactionHandler,
        IAudioService audioService,
        IOptions<BotSettings> options
    )
    {
        _logger = logger;
        _client = client;
        _interactionHandler = interactionHandler;
        _audioService = audioService;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Log += LogAsync;

        await _interactionHandler.InitialiseAsync();

        await _client.LoginAsync(TokenType.Bot, _settings.BotToken);
        await _client.StartAsync();

        _audioService.TrackEnded += OnTrackEnded;
    }

    private async Task OnTrackEnded(object sender, TrackEndedEventArgs eventArgs)
    {
        var queuedPlayer = eventArgs.Player as IQueuedLavalinkPlayer;

        if (queuedPlayer is not null && queuedPlayer.State == PlayerState.NotPlaying)
            await queuedPlayer.DisconnectAsync();
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
