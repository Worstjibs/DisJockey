using System;

namespace API.DTOs {
    public class UserDto {
        public string Username { get; set; }
        public long DiscordId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}