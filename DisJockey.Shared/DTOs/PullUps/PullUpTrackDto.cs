using DisJockey.Shared.DTOs.Shared;
using System;

namespace DisJockey.Shared.DTOs.PullUps {
    public class PullUpTrackDto {
        public DateTime DatePulled { get; set; }
        public double TimePulled { get; set; }
        public BaseMemberDto Member { get; set; }
    }
}