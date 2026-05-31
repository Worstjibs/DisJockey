using System.Collections.Concurrent;

namespace DisJockey.Infrastructure.Hubs;

public interface IUserConnectionTracker
{
    void Add(
        string userId,
        string connectionId,
        ulong? voiceChannelId = null);

    void Remove(string userId);

    UserConnection? GetConnection(string userId);
}

public sealed class UserConnectionTracker : IUserConnectionTracker
{
    private readonly ConcurrentDictionary<string, UserConnection> _connections = new();

    public void Add(
        string userId,
        string connectionId,
        ulong? voiceChannelId = null) =>
        _connections[userId] = new UserConnection(connectionId, voiceChannelId);

    public void Remove(string userId) =>
        _connections.TryRemove(userId, out _);

    public UserConnection? GetConnection(string userId) =>
        _connections.GetValueOrDefault(userId);
}

public record UserConnection(string ConnectionId, ulong? VoiceChannelId);
