using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    [Table("Users")]
    public class AppUser {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; }
        public ulong DiscordId { get; set; }
        public string AvatarUrl { get; set; }
        public ICollection<TrackPlay> Tracks { get; set; }
        public ICollection<TrackLike> Likes { get; set; }
        public ICollection<PullUp> PullUps { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}