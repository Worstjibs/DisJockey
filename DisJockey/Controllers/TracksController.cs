using System.Linq;
using System.Threading.Tasks;
using DisJockey.Shared.DTOs;
using DisJockey.Core;
using DisJockey.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Services.Interfaces;
using DisJockey.Shared.Helpers;
using DisJockey.Shared.Extensions;
using MassTransit;
using DisJockey.Shared.Events;

namespace DisJockey.Controllers;

[Authorize]
public class TracksController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBus _bus;

    public TracksController(IUnitOfWork unitOfWork, IBus bus)
    {
        _unitOfWork = unitOfWork;
        _bus = bus;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<TrackListDto>>> GetTracks([FromQuery] PaginationParams paginationParams)
    {
        var tracks = await _unitOfWork.TrackRepository.GetTracks(paginationParams);

        Response.AddPaginationHeader(tracks.CurrentPage, tracks.ItemsPerPage, tracks.TotalPages, tracks.TotalCount);

        return Ok(tracks);
    }

    [HttpGet("{discordId}")]
    public async Task<ActionResult<PagedList<TrackListDto>>> GetTrackPlaysForMember([FromQuery] PaginationParams paginationParams, ulong discordId)
    {
        var tracks = await _unitOfWork.TrackRepository.GetTrackPlaysForMember(paginationParams, discordId);

        Response.AddPaginationHeader(tracks.CurrentPage, tracks.ItemsPerPage, tracks.TotalPages, tracks.TotalCount);

        return Ok(tracks);
    }

    [HttpPost("like")]
    public async Task<ActionResult> LikeTrack(TrackLikeAddDto trackLikeDto)
    {
        var track = await _unitOfWork.TrackRepository.GetTrackByYoutubeIdAsync(trackLikeDto.YoutubeId);

        if (track == null) return BadRequest("Track does not exist");

        var discordId = User.GetDiscordId();
        if (!discordId.HasValue)
        {
            return BadRequest("Invalid DiscordId");
        }

        var user = await _unitOfWork.UserRepository.GetUserByDiscordIdAsync(discordId.Value);

        if (user == null) return Unauthorized("Invalid Token");

        var trackLike = track.Likes.FirstOrDefault(t => t.User.DiscordId == discordId);

        if (trackLike == null)
        {
            trackLike = new TrackLike
            {
                UserId = user.Id,
                TrackId = track.Id
            };
        }
        else if (trackLike.Liked == trackLikeDto.Liked)
            return BadRequest("You already like this track");

        trackLike.Liked = trackLikeDto.Liked;

        track.Likes.Add(trackLike);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Error saving like");
    }

    [HttpPost("play")]
    public async Task<ActionResult> PlayTrack(TrackPlayRequestDto trackPlayDto)
    {
        var discordId = User.GetDiscordId();

        if (!discordId.HasValue)
            return BadRequest("Invalid DiscordId");

        if (await _unitOfWork.TrackRepository.IsTrackBlacklisted(trackPlayDto.YoutubeId))
            return BadRequest("Track is blacklisted");

        var playTrackEvent = new PlayTrackEvent
        {
            DiscordId = discordId.Value,
            YoutubeId = trackPlayDto.YoutubeId,
            Queue = trackPlayDto.PlayNow
        };

        await _bus.Publish(playTrackEvent);

        return Ok();
    }

    [HttpPut("{id}/blacklist")]
    public async Task<ActionResult> BlacklistTrack(int trackId)
    {
        var track = await _unitOfWork.TrackRepository.GetTrackByIdAsync(trackId);

        if (track == null)
            return NotFound($"Track with Id {trackId} not found.");

        if (track.Blacklisted)
            return BadRequest($"Track with Id {trackId} already blacklisted.");

        track.Blacklisted = true;

        await _unitOfWork.Complete();

        return NoContent();
    }
}