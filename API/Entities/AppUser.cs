using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities {
    public class AppUser {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}