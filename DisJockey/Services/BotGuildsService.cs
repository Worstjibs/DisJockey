using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Services;

public class BotGuildsService
{
    ConcurrentBag<ulong> _botGuildIds = new ConcurrentBag<ulong>();

    public async Task<bool> IsPartOfGuildsAsync(IEnumerable<ulong> userGuildIds)
    {
        return await Task.Run(() =>
        {
            var joined = _botGuildIds.Join(userGuildIds, x => x, y => y, (x, y) => x).ToList();

            return joined.Any();
        });
    }

    public async Task AddGuildsAsync(IEnumerable<ulong> botGuilds)
    {
        await Task.Run(() =>
        {
            _botGuildIds = [.. botGuilds];
        });
    }
}
