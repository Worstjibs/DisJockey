using System;
using System.Collections.Generic;

namespace API.DTOs {
    public class MemberDto {
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime DateJoined { get; set; }
        public ICollection<MemberTrackDto> Tracks { get; set; }
    }
}