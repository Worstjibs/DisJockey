using API.DTOs.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Shared {
    public class BasePlaylistDto {
        public string YoutubeId { get; set; }
        public string Name { get; set; }
    }
}
