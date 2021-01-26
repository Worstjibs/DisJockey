using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    [Table("Tracks")]
    public class Track {
        public int Id { get; set; }
        public string YoutubeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChannelTitle { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string Thumbnail { get; set; }
        public virtual ICollection<AppUserTrack> AppUsers { get; set; }
        public virtual ICollection<TrackLikes> Likes { get; set; }
    }
}