using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models {
    [Table("TrackPlays")]
    public class AppUserTrack {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public AppUser User { get; set; }
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}