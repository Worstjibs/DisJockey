using System;
using System.Collections.Generic;

namespace API.Entities {
    public class AppUser {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
        public long DiscordId { get; set; }
        public virtual ICollection<AppUserTrack> Tracks { get; set; }
    }
}