namespace DisJockey.MassTransit.Events.BotGuilds;

public class GetBotGuildsEvent
{
    public class Response
    {
        public IEnumerable<ulong> GuidIds { get; set; } = [];
    }
}
