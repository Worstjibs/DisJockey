using DisJockey.Shared.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisJockey.Shared.DTOs.Member {
    public class MemberDetailDto: MemberListDto {
        public ICollection<BasePlaylistDto> Playlists { get; set; }
    }
}
