using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisJockey.Core {
    [Table("PullUps")]
    public class PullUp {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public double TimePulled { get; set; }
    }
}