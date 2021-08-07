using DisJockey.Shared.DTOs.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisJockey.Shared.DTOs.PullUps {
    public class PullUpDto : BaseTrackDto {
        public ICollection<PullUpTrackDto> PullUps { get; set; }
        public DateTime LastPulled { get; set; }
    }
}
