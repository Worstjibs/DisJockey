using DisJockey.Shared.DTOs.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Shared.DTOs.Shared {
    public class BasePlaylistDto {
        public string YoutubeId { get; set; }
        public string Name { get; set; }
    }
}
