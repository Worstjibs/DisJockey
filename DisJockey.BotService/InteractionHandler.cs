using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DisJockey.BotService;

internal class InteractionHandler
{
    private readonly InteractionService _interactionService;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _environment;
    private readonly BotSettings _settings;

    public InteractionHandler(
        InteractionService interactionService,
        DiscordSocketClient client,
        IServiceProvider serviceProvider,
        IHostEnvironment environment,
        IOptions<BotSettings> settings
    )
    {
        _interactionService = interactionService;
        _client = client;
        _serviceProvider = serviceProvider;
        _environment = environment;
        _settings = settings.Value;
    }

    public async Task InitialiseAsync()
    {
        _client.Ready += ReadyAsync;

        await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

        _client.InteractionCreated += HandleInteraction;
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

    private async Task ReadyAsync()
    {
        if (_environment.IsDevelopment() && _settings.GuildId.HasValue)
            await _interactionService.RegisterCommandsToGuildAsync(_settings.GuildId.Value, true);
        else
            await _interactionService.RegisterCommandsGloballyAsync(true);
    }
}
