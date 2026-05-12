using System.Linq.Expressions;
using DisJockey.Core;
using DisJockey.Shared.DTOs.PullUps;
using DisJockey.Shared.DTOs.Shared;

namespace DisJockey.Application.Mappers;

public static class PullUpMapper
{
    private static readonly Expression<Func<Track, ulong?, PullUpDto>> _expression =
        (track, discordId) => new PullUpDto
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
            LastPulled = track.PullUps.Max(x => x.CreatedOn),
            PullUps = track.PullUps
                .Select(pu => new PullUpTrackDto
                {
                    DatePulled = pu.CreatedOn,
                    TimePulled = pu.TimePulled,
                    Member = new BaseMemberDto
                    {
                        DiscordId = pu.User.DiscordId.ToString(),
                        Username = pu.User.UserName,
                        AvatarUrl = pu.User.AvatarUrl,
                        DateJoined = pu.User.CreatedOn
                    }
                })
                .ToList()
        };

    private static readonly Lazy<Func<Track, ulong?, PullUpDto>> _compiled = new(_expression.Compile);

    public static Expression<Func<Track, PullUpDto>> ToListDtoExpression(ulong? discordId)
    {
        var trackParam = _expression.Parameters[0];
        var discordIdConst = Expression.Constant(discordId, typeof(ulong?));
        var body = new SubstituteVisitor(_expression.Parameters[1], discordIdConst)
            .Visit(_expression.Body)!;
        return Expression.Lambda<Func<Track, PullUpDto>>(body, trackParam);
    }

    extension(Track track)
    {
        public PullUpDto ToPullUpDto(ulong? discordId = null) =>
            _compiled.Value(track, discordId);
    }

    private sealed class SubstituteVisitor(ParameterExpression target, Expression replacement)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == target ? replacement : base.VisitParameter(node);
    }
}
