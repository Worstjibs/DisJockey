using System;

namespace API.Entities {
    public class TrackPlayHistory {
        public int Id { get; set; }
        public TrackPlay TrackPlay { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}