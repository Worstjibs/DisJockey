namespace DisJockey.BotService;

internal class BotSettings
{
    public required string BotToken { get; set; }
    public ulong? GuildId { get; set; }
}
