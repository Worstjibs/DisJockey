namespace DisJockey.MassTransit.Events;

public class PlayTrackEvent
{
    public ulong DiscordId { get; set; }
    public string YoutubeId { get; set; }
    public bool Queue { get; set; }
}
