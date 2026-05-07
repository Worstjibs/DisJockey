namespace DisJockey.Application.Interfaces;

public interface IDiscordTrackService
{
    Task AddTrackAsync(AddTrackArgs args, string youtubeId);

    public struct AddTrackArgs
    {
        public ulong DiscordId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
    }
}