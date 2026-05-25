namespace DisJockey.Shared.Messaging.Events;

public record TrackStatusChangedEvent(
    ulong VoiceChannelId,
    TrackDetails? TrackDetails);

public record TrackDetails(string TrackName);
