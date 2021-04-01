using System;
using System.Collections.Generic;

namespace API.Entities {
    public class AppUser {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
        public ulong DiscordId { get; set; }
        public ICollection<AppUserTrack> Tracks { get; set; }
        public ICollection<TrackLike> Likes { get; set; }
    }
}