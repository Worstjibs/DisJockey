using DisJockey.Shared.DTOs.Shared;
using System.Collections.Generic;

namespace DisJockey.Shared.DTOs.Member;

public class MemberDetailDto : MemberListDto
{
    public ICollection<BasePlaylistDto> Playlists { get; set; } = [];
}
