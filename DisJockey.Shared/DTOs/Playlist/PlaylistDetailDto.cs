using DisJockey.Shared.DTOs.Shared;
using System.Collections.Generic;

namespace DisJockey.Shared.DTOs.Playlist;

public class PlaylistDetailDto : BasePlaylistDto
{
    public ICollection<BaseTrackDto> Tracks { get; set; } = [];
}
