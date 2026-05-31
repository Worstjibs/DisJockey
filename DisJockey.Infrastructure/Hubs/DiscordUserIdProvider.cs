using DisJockey.Shared.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace DisJockey.Infrastructure.Hubs;

public sealed class DiscordUserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Keyed by discord_id claim so that Clients.User(discordId) targets the
    /// correct browser connection regardless of how many tabs are open.
    /// </summary>
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.GetDiscordId()?.ToString();
    }
}
