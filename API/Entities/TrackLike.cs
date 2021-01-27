using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {

    [Table("TrackLikes")]
    public class TrackLike {
        public int Id { get; set; }
        public Track Track { get; set; }
        public int TrackId { get; set; }
        public AppUser User { get; set; }
        public int UserId { get; set; }
        public bool Liked { get; set; }
    }
}