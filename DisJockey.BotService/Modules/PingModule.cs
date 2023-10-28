using Discord.Interactions;

namespace DisJockey.BotService.Modules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Returns Pong")]
    public async Task Ping()
    {
        try
        {
            await RespondAsync("Pong!");
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
