using Discord.WebSocket;
using DisJockey.Shared.Messaging.Contracts;
using DisJockey.Shared.Messaging.Enums;
using DisJockey.Shared.Messaging.Events;

namespace DisJockey.BotService.Services.Events;

public interface IEventsPublisher
{
    Task PublishTrackPlayedAsync(
        ulong userId,
        string trackIdentifier,
        SearchMode searchMode);
}

public class EventsPublisher : IEventsPublisher
{
    private readonly DiscordSocketClient _discordClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventsPublisher(DiscordSocketClient discordClient, IServiceScopeFactory serviceScopeFactory)
    {
        _discordClient = discordClient;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task PublishTrackPlayedAsync(
        ulong userId,
        string trackIdentifier,
        SearchMode searchMode)
    {
        var user = await _discordClient.GetUserAsync(userId) as SocketUser
            ?? throw new Exception($"Discord user {userId} not found");

        var trackPlayedEvent = new TrackPlayedEvent(
            trackIdentifier,
            user.Id,
            user.GetAvatarUrl(),
            user.Username,
            searchMode);

        using var scope = _serviceScopeFactory.CreateScope();
        var messageSender = scope.ServiceProvider.GetRequiredService<IMessageSender>();

        await messageSender.SendAsync(trackPlayedEvent);
    }
}
