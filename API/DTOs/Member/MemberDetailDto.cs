using API.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Member {
    public class MemberDetailDto: MemberListDto {
        public ICollection<BasePlaylistDto> Playlists { get; set; }
    }
}
