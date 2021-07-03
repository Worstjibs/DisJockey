using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System;
using API.Helpers;
using API.Models;

namespace API.Data {
    public class TrackRepository : ITrackRepository {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TrackRepository(DataContext context, IMapper mapper) {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Track> GetTrackByIdAsync(int id) {
            return await _context.Tracks.FindAsync(id);
        }

        public async Task<Track> GetTrackByYoutubeIdAsync(string youtubeId) {
            return await _context.Tracks
                .Include(x => x.Likes).ThenInclude(x => x.User)
                .Include(x => x.TrackPlays).ThenInclude(x => x.TrackPlayHistory)
                .Include(x => x.PullUps)
                .FirstOrDefaultAsync(x => x.YoutubeId == youtubeId);
        }

        public async Task<PagedList<TrackDto>> GetTracks(PaginationParams paginationParams) {
            var userTracks = _context.Tracks.AsQueryable()
                .ProjectTo<TrackDto>(_mapper.ConfigurationProvider)                
                .OrderByDescending(x => x.LastPlayed);

            return await PagedList<TrackDto>.CreateAsync(userTracks, paginationParams.PageNumber, paginationParams.PageSize);
        }

        public void AddTrack(Track track) {
            _context.Tracks.Add(track);
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

        public async Task AddPlaylist(Playlist playlist) {
            var playlistTrackIds = playlist.Tracks.Select(x => x.Track.YoutubeId);

            var playlistTracks = await _context.Tracks
                .Include(x => x.Playlists)
                .Where(x => playlistTrackIds.Any(y => y == x.YoutubeId))
                .ToListAsync();

            var playlistToSave = new Playlist {
                Name = playlist.Name
            };
            playlistToSave.Tracks = playlistTracks.Select(x => new PlaylistTrack {
                CreatedOn = DateTime.Now,
                Playlist = playlistToSave,
                PlaylistId = playlistToSave.Id,
                Track = x,
                TrackId = x.Id
            }).ToList();

            _context.Playlists.Add(playlistToSave);

            await _context.SaveChangesAsync();
        }

        public async Task<PlaylistModel> GetPlaylist(int playlistId) {
            return await _context.Playlists.AsQueryable()
                .Where(x => x.Id == playlistId)
                .ProjectTo<PlaylistModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }
}