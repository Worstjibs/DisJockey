using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DisJockey.BotService.Hubs;
using DisJockey.Shared.Notifications;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DisJockey.BotService.Interactions;

internal class InteractionHandler
{
    private readonly InteractionService _interactionService;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly HubConnectionProvider _hubConnectionProvider;
    private readonly BotSettings _settings;

    public InteractionHandler(
        InteractionService interactionService,
        DiscordSocketClient client,
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        HubConnectionProvider hubConnectionProvider,
        IOptions<BotSettings> settings
    )
    {
        _interactionService = interactionService;
        _client = client;
        _serviceProvider = serviceProvider;
        _environment = environment;
        _hubConnectionProvider = hubConnectionProvider;
        _settings = settings.Value;
    }

    public async Task InitialiseAsync()
    {
        _client.Ready += ReadyAsync;

        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

        _client.InteractionCreated += HandleInteraction;

        _client.UserVoiceStateUpdated += HandleUserVoiceStatusUpdate;

        await _hubConnectionProvider.InitializeAsync();
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    default:
                        break;
                }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }

    private async Task HandleUserVoiceStatusUpdate(
        SocketUser user,
        SocketVoiceState before,
        SocketVoiceState after)
    {
        if (user.IsBot)
        {
            return;
        }

        var notification = new UserVoiceStateNotification
        {
            DiscordId = user.Id
        };

        if (after.VoiceChannel is not null)
        {
            notification.VoiceState = VoiceState.Connected;
            notification.VoiceChannel = new VoiceChannelInfo
            {
                Id = after.VoiceChannel.Id,
                Name = after.VoiceChannel.Name,
                ServerId = after.VoiceChannel.Guild.Id,
                ServerName = after.VoiceChannel.Guild.Name
            };
        }

        await _hubConnectionProvider.InvokeAsync(
            "UserVoiceStateChanged", 
            notification);
    }

    private async Task ReadyAsync()
    {
        if (_environment.IsDevelopment() && _settings.GuildId.HasValue)
            await _interactionService.RegisterCommandsToGuildAsync(_settings.GuildId.Value, true);
        else
            await _interactionService.RegisterCommandsGloballyAsync(true);
    }
}
