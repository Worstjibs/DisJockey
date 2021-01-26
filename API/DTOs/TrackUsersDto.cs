using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class TrackUsersDto
    {
        public string YoutubeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public ICollection<UserDto> Users { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChannelTitle { get; set; }
        public string SmallThumbnail { get; set; }
        public string MediumThumbnail { get; set; }
        public string LargeThumbnail { get; set; }
    }
}