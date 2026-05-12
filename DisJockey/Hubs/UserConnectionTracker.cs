using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DisJockey.Hubs;

public interface IUserConnectionTracker
{
    void Add(string userId, string connectionId);
    void Remove(string userId);
    string? GetConnection(string userId);
}

public sealed class UserConnectionTracker : IUserConnectionTracker
{
    private readonly ConcurrentDictionary<string, string> _connections = new();

    public void Add(string userId, string connectionId) =>
        _connections[userId] = connectionId;

    public void Remove(string userId) =>
        _connections.TryRemove(userId, out _);

    public string? GetConnection(string userId) =>
        _connections.GetValueOrDefault(userId);
}
