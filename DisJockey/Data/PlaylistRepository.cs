using DisJockey.Shared.DTOs.Playlist;
using DisJockey.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.DTOs.Shared;
using DisJockey.Shared.Helpers;

namespace DisJockey.Data {
    public class PlaylistRepository : IPlaylistRepository {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PlaylistRepository(DataContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddMissingTracks(IList<PlaylistTrack> playlistTracks) {
            var trackIds = playlistTracks.Select(x => x.Track.YoutubeId);

            var existingTrackIds = await _context.Tracks.AsNoTracking()
                .Where(x => trackIds.Contains(x.YoutubeId)).Select(x => x.YoutubeId)
                .ToListAsync();

            var missingTracks = playlistTracks.Where(x => !existingTrackIds.Any(y => y == x.Track.YoutubeId)).Select(x => x.Track);

            foreach (var track in missingTracks) {
                _context.Tracks.Add(track);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Playlist> AddPlaylist(Playlist playlist, AppUser user) {
            var playlistTrackIds = playlist.Tracks.Select(x => x.Track.YoutubeId);

            var playlistTracks = await _context.Tracks
                .Include(x => x.Playlists)
                .Where(x => playlistTrackIds.Any(y => y == x.YoutubeId))
                .ToListAsync();

            var playlistToSave = new Playlist {
                Name = playlist.Name,
                YoutubeId = playlist.YoutubeId
            };
            playlistToSave.Tracks = playlistTracks.Select(x => new PlaylistTrack {
                CreatedOn = DateTime.Now,
                CreatedBy = user,
                Playlist = playlistToSave,
                PlaylistId = playlistToSave.Id,
                Track = x,
                TrackId = x.Id
            }).ToList();

            user.Playlists.Add(playlistToSave);

            await _context.SaveChangesAsync();

            return playlistToSave;
        }

        public async Task<bool> CheckPlaylistExists(string playlistId) {
            return await _context.Playlists.AsQueryable().AnyAsync(x => x.YoutubeId == playlistId);
        }

        public async Task<PagedList<BaseTrackDto>> GetPlaylistTracks(PaginationParams paginationParams, string youtubeId) {
            var source = _context.PlaylistTracks.AsNoTracking()
                .Where(x => x.Playlist.YoutubeId == youtubeId)
                .ProjectTo<BaseTrackDto>(_mapper.ConfigurationProvider)
                .OrderBy(x => x.Title);

            return await PagedList<BaseTrackDto>.CreateAsync(source, paginationParams.PageNumber, paginationParams.PageSize);
        }
    }
}
