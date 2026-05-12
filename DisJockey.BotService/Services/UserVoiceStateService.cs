using Discord.WebSocket;

namespace DisJockey.BotService.Services;

public interface IUserVoiceStateService
{
    Task<UserVoiceState?> GetUserVoiceStateAsync(ulong userId, CancellationToken cancellationToken = default);
}

public class UserVoiceStateService : IUserVoiceStateService
{
    private readonly DiscordSocketClient _discordSocketClient;

    public UserVoiceStateService(DiscordSocketClient discordSocketClient)
    {
        _discordSocketClient = discordSocketClient;
    }

    public async Task<UserVoiceState?> GetUserVoiceStateAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await _discordSocketClient.GetUserAsync(userId) is not SocketUser user)
        {
            return null;
        }

        var voiceChannel = user.MutualGuilds
                            .SelectMany(x => x.VoiceChannels)
                            .FirstOrDefault(x => x.ConnectedUsers.Any(u => u.Id == userId));

        if (voiceChannel is null)
        {
            return null;
        }

        var guild = voiceChannel.Guild;

        return new(
            guild.Id,
            guild.Name,
            voiceChannel.Id,
            voiceChannel.Name);
    }
}

public record UserVoiceState(
    ulong ServerId,
    string ServerName,
    ulong VoiceChannelId,
    string VoiceChannelName);