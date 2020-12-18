using System.Collections.Generic;

namespace API.DTOs {
    public class MemberDto {
        public string Username { get; set; }
        public ICollection<TrackDto> Tracks { get; set; }
    }
}