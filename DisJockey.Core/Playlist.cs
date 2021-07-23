using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisJockey.Core {
    [Table("Playlists")]
    public class Playlist {
        public int Id { get; set; }
        public string Name { get; set; }
        public string YoutubeId { get; set; }
        public IList<PlaylistTrack> Tracks { get; set; }
    }
}