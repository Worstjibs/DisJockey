namespace DisJockey.Shared.DTOs {
    public class TrackUserLikeDto {
        public string Username { get; set; }
        public ulong DiscordId { get; set; }
        public bool Liked { get; set; }
    }
}