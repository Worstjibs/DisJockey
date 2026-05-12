namespace DisJockey.Shared.Messaging.Events;

public class PlayTrackEvent
{
    public ulong DiscordId { get; set; }
    public required string YoutubeId { get; set; }
    public bool Queue { get; set; }
}
