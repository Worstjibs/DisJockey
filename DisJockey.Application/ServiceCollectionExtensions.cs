using DisJockey.Application.Contracts;
using DisJockey.Application.Features.Members.Queries.AllMembers;
using DisJockey.Application.Features.Members.Queries.GetMember;
using DisJockey.Application.Features.Tracks.Commands.BlacklistTrack;
using DisJockey.Application.Features.Tracks.Commands.LikeTrack;
using DisJockey.Application.Features.Tracks.Commands.PlayTrack;
using DisJockey.Application.Features.Tracks.Queries.AllTracks;
using DisJockey.Application.Features.Tracks.Queries.TracksForMember;
using DisJockey.Application.Profiles;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using ErrorOr;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServicesV2()
        {
            services.AddAutoMapper(configuration =>
            {
                configuration.AddProfile<AutoMapperProfiles>();
            });

            services.RegisterHandlers();

            return services;
        }

        private IServiceCollection RegisterHandlers()
        {
            services.AddScoped<IMediator, Mediator>();

            services.AddScoped<IRequestHandler<AllMembersQuery, IEnumerable<MemberListDto>>, AllMembersHandler>();
            services.AddScoped<IRequestHandler<GetMemberQuery, MemberDetailDto?>, GetMemberHandler>();
            services.AddScoped<IRequestHandler<BlacklistTrackCommand, ErrorOr<Success>>, BlacklistTrackHandler>();
            services.AddScoped<IRequestHandler<LikeTrackCommand, ErrorOr<Success>>, LikeTrackHandler>();
            services.AddScoped<IRequestHandler<PlayTrackCommand, ErrorOr<Success>>, PlayTrackHandler>();
            services.AddScoped<IRequestHandler<AllTracksQuery, PagedList<TrackListDto>>, AllTracksHandler>();
            services.AddScoped<IRequestHandler<TracksForMemberQuery, PagedList<TrackListDto>>, TracksForMemberHandler>();

            return services;
        }
    }
}
