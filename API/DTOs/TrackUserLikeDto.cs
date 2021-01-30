namespace API.DTOs {
    public class TrackUserLikeDto {
        public string Username { get; set; }
        public long DiscordId { get; set; }
        public bool Liked { get; set; }
    }
}