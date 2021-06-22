using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    [Table("Playlists")]
    public class Playlist {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<PlaylistTrack> Tracks { get; set; }
    }
}