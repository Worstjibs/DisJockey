﻿namespace DisJockey.Shared.Events;

public class PlayTrackEvent
{
    public ulong DiscordId { get; set; }
    public string YoutubeId { get; set; }
}
