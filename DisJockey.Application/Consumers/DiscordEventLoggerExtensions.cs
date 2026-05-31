using Microsoft.Extensions.Logging;

namespace DisJockey.Application.Consumers;

public static partial class DiscordEventLoggerExtensions
{
    [LoggerMessage(
        LogLevel.Debug,
        "Received {EventName} for VoiceChannel {VoiceChannelId}")]
    public static partial void LogDiscordEventReceived(
        this ILogger logger,
        string eventName,
        ulong voiceChannelId);

    [LoggerMessage(
        LogLevel.Debug,
        "Received {EventName} for Discord user {DiscordId}")]
    public static partial void LogDiscordUserEventReceived(
        this ILogger logger,
        string eventName,
        ulong discordId);
}
