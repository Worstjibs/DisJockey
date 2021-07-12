﻿using API.DTOs.Playlist;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data {
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

        public async Task<PlaylistDetailDto> GetPlaylistById(int id) {
            return await _context.Playlists.AsQueryable()
                .Where(x => x.Id == id)
                .ProjectTo<PlaylistDetailDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PlaylistDetailDto> GetPlaylistByYoutubeId(string youtubeId) {
            return await _context.Playlists.AsQueryable()
                .Where(x => x.YoutubeId == youtubeId)
                .ProjectTo<PlaylistDetailDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }
}
