using DisJockey.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DisJockey.Hubs;

public class TrackControlHub : Hub<ITrackControlHub>
{
    public async Task UserVoiceStateChanged(ulong discordId, ulong? voiceChannelId, VoiceState state)
    {
        await Clients.All.SendMessageAsync($"User {discordId} {state} to channel {voiceChannelId}");
    }
}

public interface ITrackControlHub
{
    Task SendMessageAsync(string message);
}
