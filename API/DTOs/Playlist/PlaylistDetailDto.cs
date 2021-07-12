using API.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Playlist {
    public class PlaylistDetailDto : BasePlaylistDto {
        public ICollection<BaseTrackDto> Tracks { get; set; }
    }
}
