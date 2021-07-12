using API.DTOs.Shared;
using System;
using System.Collections.Generic;

namespace API.DTOs.Track {
    public class TrackListDto : BaseTrackDto {
        public ICollection<TrackPlayDto> Users { get; set; }
        public ICollection<TrackUserLikeDto> UserLikes { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public bool? LikedByUser { get; set; } = null;
        public DateTime LastPlayed { get; set; }
    }
}