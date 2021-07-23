using System;
using System.Collections.Generic;

namespace DisJockey.Shared.DTOs.Member {
    public class MemberListDto {
        public string DiscordId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime DateJoined { get; set; }
        public int TracksPlayed { get; set; }
    }
}