using DisJockey.Shared.DTOs.Shared;
using DisJockey.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Shared.DTOs.Playlist {
    public class PlaylistDetailDto : BasePlaylistDto {
        public ICollection<BaseTrackDto> Tracks { get; set; }
    }
}
