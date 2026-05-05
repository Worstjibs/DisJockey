using System.Linq.Expressions;
using DisJockey.Core;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.DTOs;

namespace DisJockey.Application.Mappers;

public static class TrackMapper
{
    private static readonly Expression<Func<Track, ulong?, TrackListDto>> _expression =
        (track, discordId) => new TrackListDto
        {
            YoutubeId = track.YoutubeId,
            CreatedOn = track.CreatedOn,
            Title = track.Title,
            Description = track.Description,
            ChannelTitle = track.ChannelTitle,
            SmallThumbnail = track.SmallThumbnail,
            MediumThumbnail = track.MediumThumbnail,
            LargeThumbnail = track.LargeThumbnail,
            Likes = track.Likes.Count(x => x.Liked == true),
            Dislikes = track.Likes.Count(x => x.Liked == false),
            LikedByUser = track.Likes
                .Where(x => x.User.DiscordId == discordId)
                .Select(x => (bool?)x.Liked)
                .FirstOrDefault(),
            Users = track.TrackPlays
                .OrderByDescending(tp => tp.LastPlayed)
                .Select(tp => new TrackPlayDto
                {
                    Username = tp.User.UserName,
                    DiscordId = tp.User.DiscordId,
                    TimesPlayed = tp.TrackPlayHistory.Count,
                    LastPlayed = tp.LastPlayed,
                    FirstPlayed = tp.CreatedOn,
                    History = tp.TrackPlayHistory
                        .Select(h => new TrackPlayHistoryDto { CreatedOn = h.CreatedOn })
                        .ToList()
                })
                .ToList(),
            UserLikes = track.Likes
                .Select(l => new TrackUserLikeDto
                {
                    Username = l.User.UserName,
                    DiscordId = l.User.DiscordId,
                    Liked = l.Liked
                })
                .ToList(),
            LastPlayed = track.TrackPlays.Count > 0
                ? track.TrackPlays.Max(x => x.LastPlayed)
                : DateTime.MinValue
        };

    private static readonly Lazy<Func<Track, ulong?, TrackListDto>> _compiled = new(_expression.Compile);

    public static Expression<Func<Track, TrackListDto>> ToListDtoExpression(ulong? discordId = null)
    {
        var trackParam = _expression.Parameters[0];
        var discordIdConst = Expression.Constant(discordId, typeof(ulong?));
        var body = new SubstituteVisitor(_expression.Parameters[1], discordIdConst)
            .Visit(_expression.Body)!;
        return Expression.Lambda<Func<Track, TrackListDto>>(body, trackParam);
    }

    extension(Track track)
    {
        public TrackListDto ToListDto(ulong? discordId = null) =>
            _compiled.Value(track, discordId);
    }

    private sealed class SubstituteVisitor(ParameterExpression target, Expression replacement)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == target ? replacement : base.VisitParameter(node);
    }
}
