using System.Collections.Generic;

namespace API.Models {
    public class Playlist {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<PlaylistTrack> Tracks { get; set; }
    }
}