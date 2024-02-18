using DisJockey.MassTransit.Enums;

namespace DisJockey.MassTransit.Events;

public class TrackPlayedEvent
{
    public SearchMode SearchMode { get; set; }
    public string TrackId { get; set; }
    public ulong DiscordId { get; set; }
    public string AvatarUrl { get; set; }
    public string UserName { get; set; }
}
