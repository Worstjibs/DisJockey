using API.DTOs.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Playlist {
    public class PlaylistDto {
        public string YoutubeId { get; set; }
        public string Name { get; set; }
        public ICollection<BaseTrackDto> Tracks { get; set; }
    }
}
