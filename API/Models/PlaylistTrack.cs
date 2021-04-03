using System;

namespace API.Models {
    public class PlaylistTrack {
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }
        public DateTime CreatedOn { get; set; }
        public AppUser CreatedBy { get; set; }
    }
}