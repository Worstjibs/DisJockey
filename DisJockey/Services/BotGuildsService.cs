using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Services;

public class BotGuildsService
{
    ConcurrentBag<ulong> _botGuildIds = new ConcurrentBag<ulong>();
    ConcurrentDictionary<ulong, IEnumerable<ulong>> _cachedUserGuilds = new ConcurrentDictionary<ulong, IEnumerable<ulong>>();

    public async Task<IEnumerable<ulong>?> TryGetUserGuilds(ulong userId)
    {
        return await Task.Run(() =>
        {
            if (!_cachedUserGuilds.TryGetValue(userId, out var guildIds))
                return null;

            return guildIds;
        });
    }

    public async Task AddUserGuildsAsync(ulong userId, IEnumerable<ulong> guildIds)
    {
        await Task.Run(() =>
        {
            _cachedUserGuilds.TryAdd(userId, guildIds);
        });
    }

    public async Task<bool> IsPartOfGuildsAsync(IEnumerable<ulong> userGuildIds)
    {
        return await Task.Run(() =>
        {
            var joined = _botGuildIds.Join(userGuildIds, x => x, y => y, (x, y) => x).ToList();

            return joined.Any();
        });
    }

    public async Task AddBotGuildsAsync(IEnumerable<ulong> botGuilds)
    {
        await Task.Run(() =>
        {
            _botGuildIds = [.. botGuilds];
        });
    }
}
