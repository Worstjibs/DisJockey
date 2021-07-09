using System;
using System.Collections.Generic;

namespace API.DTOs.Member {
    public class MemberDto {
        public string DiscordId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime DateJoined { get; set; }
        public ICollection<MemberTrackDto> Tracks { get; set; }
    }
}