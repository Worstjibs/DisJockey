using System;
using System.Collections.Generic;

namespace API.Models {
    public class AppUser {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; }
        public ulong DiscordId { get; set; }
        public ICollection<AppUserTrack> Tracks { get; set; }
        public ICollection<TrackLike> Likes { get; set; }
    }
}