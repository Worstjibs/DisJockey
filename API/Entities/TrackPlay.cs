using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    [Table("TrackPlays")]
    public class TrackPlay {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser User { get; set; }
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime LastPlayed { get; set; }
        public ICollection<TrackPlayHistory> TrackPlayHistory { get; set; }
    }
}