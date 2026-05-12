using DisJockey.BotService.Keycloak;
using Microsoft.AspNetCore.SignalR.Client;

namespace DisJockey.BotService.Hubs;

internal sealed class HubConnectionProvider
{
    private readonly ILogger<HubConnectionProvider> _logger;
    private readonly KeycloakTokenService _keycloakTokenService;
    private readonly IHttpMessageHandlerFactory _httpMessageHandlerFactory;
    private readonly HubConnection _connection;

    public HubConnectionProvider(
        ILogger<HubConnectionProvider> logger,
        KeycloakTokenService keycloakTokenService,
        IHttpMessageHandlerFactory httpMessageHandlerFactory)
    {
        _logger = logger;
        _keycloakTokenService = keycloakTokenService;
        _httpMessageHandlerFactory = httpMessageHandlerFactory;
        _connection = CreateHubConnection();
    }

    private HubConnection CreateHubConnection()
    {
        var connection = new HubConnectionBuilder()
                            .WithUrl(
                                "https+http://api/hubs/track-control",
                                options =>
                                {
                                    options.HttpMessageHandlerFactory = _ => _httpMessageHandlerFactory.CreateHandler();
                                    options.AccessTokenProvider = () => _keycloakTokenService.GetAccessTokenAsync();
                                })
                            .WithAutomaticReconnect(
                                [
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromSeconds(15),
                                    TimeSpan.FromSeconds(30),
                                    TimeSpan.FromSeconds(60)
                                ])
                            .WithServerTimeout(TimeSpan.FromMinutes(10))
                            .Build();

        connection.Reconnecting += error =>
        {
            _logger.LogWarning("Reconnecting...");
            return Task.CompletedTask;
        };

        connection.Reconnected += connectionId =>
        {
            _logger.LogInformation("Reconnected");
            return Task.CompletedTask;
        };

        connection.Closed += async error =>
        {
            _logger.LogWarning("Connection closed");

            // Important: delay before retrying
            await Task.Delay(TimeSpan.FromSeconds(30));

            await TryStartAsync();
        };

        return connection;
    }

    public async Task InitializeAsync()
    {
        await TryStartAsync();
    }

    public async Task InvokeAsync(string methodName, params object?[] args)
    {
        if (_connection.State is HubConnectionState.Connected)
        {
            await _connection.InvokeCoreAsync(methodName, args);
        }
    }

    private async Task TryStartAsync()
    {
        var delay = TimeSpan.FromSeconds(5);

        while (true)
        {
            try
            {
                _logger.LogDebug("Attempting connection to hub");

                await _connection.StartAsync();

                _logger.LogInformation("Successfully connected to Hub: {ConnectionId}", _connection.ConnectionId);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to connect to Hub, delaying for {Delay} seconds", delay.Seconds);
                await Task.Delay(delay);
                delay = TimeSpan.FromSeconds(Math.Min(delay.TotalSeconds * 2, 300));
            }
        }
    }
}
