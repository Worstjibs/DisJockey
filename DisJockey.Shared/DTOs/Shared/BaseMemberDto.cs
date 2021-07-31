using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisJockey.Shared.DTOs.Shared {
    public class BaseMemberDto {
        public string DiscordId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime DateJoined { get; set; }
    }
}
