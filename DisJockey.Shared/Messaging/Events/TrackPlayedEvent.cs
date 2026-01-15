using DisJockey.Shared.Messaging.Enums;

namespace DisJockey.Shared.Messaging.Events;

public record TrackPlayedEvent(
    string TrackId,
    ulong DiscordId,
    string AvatarUrl,
    string UserName,
    SearchMode SearchMode);
