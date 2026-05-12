using System.Collections.Generic;

namespace DisJockey.Shared.Messaging.Events.BotGuilds;

public class GetBotGuildsEvent
{
    public class Response
    {
        public IEnumerable<ulong> GuildIds { get; set; } = [];
    }
}
