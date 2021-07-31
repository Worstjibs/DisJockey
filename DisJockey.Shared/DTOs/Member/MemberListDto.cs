using DisJockey.Shared.DTOs.Shared;
using System;
using System.Collections.Generic;

namespace DisJockey.Shared.DTOs.Member {
    public class MemberListDto : BaseMemberDto {
        public int TracksPlayed { get; set; }
    }
}