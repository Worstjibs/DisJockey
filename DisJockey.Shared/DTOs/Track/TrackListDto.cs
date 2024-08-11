using DisJockey.Shared.DTOs.Shared;
using System;
using System.Collections.Generic;

namespace DisJockey.Shared.DTOs.Track;

public class TrackListDto : BaseTrackDto
{
    public ICollection<TrackPlayDto> Users { get; set; }
    public ICollection<TrackUserLikeDto> UserLikes { get; set; }
    public DateTime LastPlayed { get; set; }
}