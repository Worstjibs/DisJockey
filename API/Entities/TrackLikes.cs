using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {

    [Table("TrackLikes")]
    public class TrackLikes {
        public int Id { get; set; }
        public Track Track { get; set; }
        public AppUser User { get; set; }
        public bool Liked { get; set; }
    }
}