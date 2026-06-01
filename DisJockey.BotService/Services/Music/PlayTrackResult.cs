namespace DisJockey.BotService.Services.Music;

public record PlayTrackResult(string Message, string? TrackIdentifier)
{
    public bool IsSuccess => TrackIdentifier is not null;
}
