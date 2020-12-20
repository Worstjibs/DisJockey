using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    [Table("Tracks")]
    public class Track {
        public int Id { get; set; }
        public string YoutubeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual ICollection<AppUser> AppUsers { get; set; }
        public virtual ICollection<TrackLikes> Likes { get; set; }
    }
}