using DisJockey.Core;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Shared.DTOs.Shared;

namespace DisJockey.Application.Mappers;

public static class MemberMapper
{
    extension(AppUser user)
    {
        public MemberListDto ToListDto() => new()
        {
            DiscordId = user.DiscordId.ToString(),
            Username = user.UserName,
            AvatarUrl = user.AvatarUrl,
            DateJoined = user.CreatedOn,
            TracksPlayed = user.Tracks.Count
        };

        public MemberDetailDto ToDetailDto() => new()
        {
            DiscordId = user.DiscordId.ToString(),
            Username = user.UserName,
            AvatarUrl = user.AvatarUrl,
            DateJoined = user.CreatedOn,
            TracksPlayed = user.Tracks.Count,
            Playlists = user.Playlists
                .Select(p => new BasePlaylistDto { YoutubeId = p.YoutubeId, Name = p.Name })
                .ToList()
        };
    }
}
