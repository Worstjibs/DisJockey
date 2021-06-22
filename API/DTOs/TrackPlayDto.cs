using System;
using System.Collections.Generic;

namespace API.DTOs {
    public class TrackPlayDto {
        public string Username { get; set; }
        public ulong DiscordId { get; set; }
        public DateTime FirstPlayed { get; set; }
        public int TimesPlayed { get; set; }
        public DateTime LastPlayed { get; set; }
        public ICollection<TrackPlayHistoryDto> History { get; set; }
    }
}