using System;

namespace API.DTOs {
    public class TrackUserDto {
        public string Username { get; set; }
        public ulong DiscordId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TimesPlayed { get; set; }
        public DateTime LastPlayed { get; set; }
    }
}