using System;

namespace DisJockey.Shared.DTOs.Member {
    public class MemberTrackDto {
        public string YoutubeId { get; set; }
        public DateTime FirstPlayed { get; set; }
        public DateTime LastPlayed { get; set; }
        public int TimesPlayed { get; set; }
    }
}