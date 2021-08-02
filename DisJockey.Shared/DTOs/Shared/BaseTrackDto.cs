using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Shared.DTOs.Shared {
    public class BaseTrackDto {
        public string YoutubeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChannelTitle { get; set; }
        public string SmallThumbnail { get; set; }
        public string MediumThumbnail { get; set; }
        public string LargeThumbnail { get; set; }
        public bool? LikedByUser { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
