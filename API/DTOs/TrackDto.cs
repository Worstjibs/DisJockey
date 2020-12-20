using System;

namespace API.DTOs {
    public class TrackDto {
        public string YoutubeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}