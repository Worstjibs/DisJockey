using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models {
    [Table("Tracks")]
    public class Track {
        public int Id { get; set; }
        public string YoutubeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChannelTitle { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string SmallThumbnail { get; set; }
        public string MediumThumbnail { get; set; }
        public string LargeThumbnail { get; set; }
        public ICollection<AppUserTrack> AppUsers { get; set; }
        public ICollection<TrackLike> Likes { get; set; }
        public ICollection<PlaylistTrack> Playlists { get; set; }
    }
}