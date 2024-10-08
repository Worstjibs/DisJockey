using DisJockey.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.PullUps;
using DisJockey.Shared.Extensions;
using Microsoft.AspNetCore.Http;

namespace DisJockey.Infrastructure.Persistence.Repositories;

public class TrackRepository : BaseRepository, ITrackRepository
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContext;

    private ulong? DiscordId;

    public TrackRepository(DataContext context, IMapper mapper, IHttpContextAccessor httpContext) : base(context)
    {
        _mapper = mapper;
        _httpContext = httpContext;

        DiscordId = _httpContext.HttpContext?.User.GetDiscordId();
    }

    public async Task<Track?> GetTrackByIdAsync(int id)
    {
        return await _context.Tracks.FindAsync(id);
    }

    public async Task<Track?> GetTrackByYoutubeIdAsync(string youtubeId)
    {
        return await _context.Tracks
            .IgnoreQueryFilters()
            .Include(x => x.Likes).ThenInclude(x => x.User)
            .Include(x => x.TrackPlays).ThenInclude(x => x.TrackPlayHistory)
            .Include(x => x.PullUps)
            .FirstOrDefaultAsync(x => x.YoutubeId == youtubeId);
    }

    public async Task<PagedList<TrackListDto>> GetTracks(PaginationParams paginationParams)
    {

        var query = _context.Tracks.AsNoTracking()
            .Include(x => x.TrackPlays)
            .Where(x => x.TrackPlays.Count > 0)
            .ProjectTo<TrackListDto>(_mapper.ConfigurationProvider, new { DiscordId });

        return await CreatePagedList(paginationParams, query);
    }

    public async Task<PagedList<TrackListDto>> GetTrackPlaysForMember(PaginationParams paginationParams, ulong discordId)
    {
        var query = _context.Tracks.AsNoTracking()
            .Where(x => x.TrackPlays.Any(tp => tp.User.DiscordId == discordId))
            .ProjectTo<TrackListDto>(_mapper.ConfigurationProvider, new { DiscordId });

        return await CreatePagedList(paginationParams, query);
    }

    public void AddTrack(Track track)
    {
        _context.Tracks.Add(track);
    }

    public async Task<PagedList<PullUpDto>> GetPullUpsForMember(PaginationParams paginationParams, ulong discordId)
    {
        var query = _context.Tracks.AsNoTracking()
            .Where(x => x.PullUps.Any(tp => tp.User.DiscordId == discordId))
            .ProjectTo<PullUpDto>(_mapper.ConfigurationProvider, new { DiscordId });

        query = paginationParams.SortBy switch
        {
            "title" => query.OrderBy(x => x.Title),
            "firstPulled" => query.OrderBy(x => x.LastPulled),
            _ => query.OrderByDescending(x => x.LastPulled),
        };

        return await PagedList<PullUpDto>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<IEnumerable<TrackListDto>> GetTracksByYouTubeIdAsync(IEnumerable<string> youTubeIds)
    {
        var query = await _context.Tracks.AsNoTracking()
            .Where(x => youTubeIds.Contains(x.YoutubeId))
            .ProjectTo<TrackListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return query;
    }

    public async Task<bool> IsTrackBlacklisted(string youtubeId)
    {
        return await _context.Tracks
            .IgnoreQueryFilters()
            .AsQueryable()
            .Where(x => x.YoutubeId == youtubeId)
            .Select(x => x.Blacklisted)
            .FirstOrDefaultAsync();
    }

    private static async Task<PagedList<TrackListDto>> CreatePagedList(PaginationParams paginationParams, IQueryable<TrackListDto> query)
    {
        query = paginationParams.SortBy switch
        {
            "title" => query.OrderBy(x => x.Title),
            "firstPlayed" => query.OrderBy(x => x.LastPlayed),
            _ => query.OrderByDescending(x => x.LastPlayed),
        };

        return await PagedList<TrackListDto>.CreateAsync(query, paginationParams.PageNumber, paginationParams.PageSize);
    }
}