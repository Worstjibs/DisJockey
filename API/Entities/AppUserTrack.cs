using System;

namespace API.Entities {
    public class AppUserTrack {
        public int AppUserId { get; set; }
        public AppUser User { get; set; }
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int TimesPlayed { get; set; } = 1;
    }
}