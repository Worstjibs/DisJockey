using DisJockey.Shared.DTOs.Shared;
using System;
using System.Collections.Generic;

namespace DisJockey.Shared.DTOs.PullUps;

public class PullUpDto : BaseTrackDto
{
    public ICollection<PullUpTrackDto> PullUps { get; set; }
    public DateTime LastPulled { get; set; }
}
