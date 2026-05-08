namespace DisJockey.Shared.Notifications;

public class UserVoiceStateNotification
{
    public ulong DiscordId { get; set; }
    public VoiceState VoiceState { get; set; } = VoiceState.Disconnected;
    public VoiceChannelInfo? VoiceChannel { get; set; }
}

public class VoiceChannelInfo
{
    public required ulong Id { get; set; }
    public required string Name { get; set; }
    public required ulong ServerId { get; set; }
    public required string ServerName { get; set; }

}

public enum VoiceState
{
    Connected,
    Disconnected
}