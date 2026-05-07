using DisJockey.Core;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Playlists;

public class GetPlaylistTracksTests : IntegrationTestBase
{
    public GetPlaylistTracksTests(DisJockeyApiFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPlaylistTracks_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.GetAsync(
            "/api/playlists/some-playlist-id",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPlaylistTracks_GivenPlaylistWithNoTracks_ReturnsEmptyPagedList()
    {
        // Arrange
        _httpClient.AsUser();

        var playlist = new Playlist
        {
            Name = "Empty Playlist",
            YoutubeId = "PLempty001",
            Tracks = []
        };

        await InsertEntityAsync(playlist);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<TrackListDto>>(
            "/api/playlists/PLempty001",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    [Fact]
    public async Task GetPlaylistTracks_GivenTracksInPlaylist_ReturnsCorrectTracks()
    {
        // Arrange
        _httpClient.AsUser();

        var createdDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "PlaylistUser",
            DiscordId = 200000000000000001,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var track1 = new Track
        {
            YoutubeId = "yt_pl_track_1",
            Title = "Alpha Track",
            Description = "Description 1",
            ChannelTitle = "Channel 1",
            CreatedOn = createdDate,
            SmallThumbnail = "http://example.com/small1.jpg",
            MediumThumbnail = "http://example.com/medium1.jpg",
            LargeThumbnail = "http://example.com/large1.jpg",
            Likes = [],
            PullUps = []
        };

        var track2 = new Track
        {
            YoutubeId = "yt_pl_track_2",
            Title = "Bravo Track",
            Description = "Description 2",
            ChannelTitle = "Channel 2",
            CreatedOn = createdDate,
            SmallThumbnail = "http://example.com/small2.jpg",
            MediumThumbnail = "http://example.com/medium2.jpg",
            LargeThumbnail = "http://example.com/large2.jpg",
            Likes = [],
            PullUps = []
        };

        var playlist = new Playlist
        {
            Name = "Test Playlist",
            YoutubeId = "PLtest002",
            Tracks =
            [
                new PlaylistTrack { Track = track1, CreatedOn = createdDate, CreatedBy = user },
                new PlaylistTrack { Track = track2, CreatedOn = createdDate, CreatedBy = user }
            ]
        };

        await AddUserAndPlaylist(user, playlist);

        // Act — results are ordered by title ascending
        var result = await _httpClient.GetFromJsonAsync<PagedList<TrackListDto>>(
            "/api/playlists/PLtest002",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.TotalItems.ShouldBe(2);
        result.Items.Select(x => x.YoutubeId).ShouldBe(["yt_pl_track_1", "yt_pl_track_2"]);
    }

    [Fact]
    public async Task GetPlaylistTracks_DoesNotReturnTracksFromOtherPlaylists()
    {
        // Arrange
        _httpClient.AsUser();

        var createdDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "IsolationUser",
            DiscordId = 200000000000000002,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var targetTrack = new Track
        {
            YoutubeId = "yt_target_playlist",
            Title = "Target Playlist Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = []
        };

        var otherTrack = new Track
        {
            YoutubeId = "yt_other_playlist",
            Title = "Other Playlist Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = []
        };

        var targetPlaylist = new Playlist
        {
            Name = "Target Playlist",
            YoutubeId = "PLtarget003",
            Tracks = [new PlaylistTrack { Track = targetTrack, CreatedOn = createdDate, CreatedBy = user }]
        };

        var otherPlaylist = new Playlist
        {
            Name = "Other Playlist",
            YoutubeId = "PLother003",
            Tracks = [new PlaylistTrack { Track = otherTrack, CreatedOn = createdDate, CreatedBy = user }]
        };

        await AddUserAndPlaylists(user, [targetPlaylist, otherPlaylist]);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<TrackListDto>>(
            "/api/playlists/PLtarget003",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.TotalItems.ShouldBe(1);
        result.Items.Single().YoutubeId.ShouldBe("yt_target_playlist");
    }

    [Fact]
    public async Task GetPlaylistTracks_GivenPageNumberAndPageSize_ReturnsPaginatedMetadata()
    {
        // Arrange
        _httpClient.AsUser();

        var createdDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "PaginationPlaylistUser",
            DiscordId = 200000000000000003,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var tracks = Enumerable.Range(1, 7).Select(i => new Track
        {
            YoutubeId = $"yt_pl_page_{i}",
            Title = $"Pagination Track {i:D2}",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = []
        }).ToArray();

        var playlist = new Playlist
        {
            Name = "Pagination Playlist",
            YoutubeId = "PLpagination004",
            Tracks = tracks.Select(t => new PlaylistTrack { Track = t, CreatedOn = createdDate, CreatedBy = user }).ToList()
        };

        await AddUserAndPlaylist(user, playlist);

        // Act — page 2 of 3 (7 total items, page size 3)
        var result = await _httpClient.GetFromJsonAsync<PagedList<TrackListDto>>(
            "/api/playlists/PLpagination004?pageNumber=2&pageSize=3",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.CurrentPage.ShouldBe(2);
        result.ItemsPerPage.ShouldBe(3);
        result.TotalItems.ShouldBe(7);
        result.TotalPages.ShouldBe(3);
        result.Items.Count().ShouldBe(3);
    }

    private async Task AddUserAndPlaylist(AppUser user, Playlist playlist)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Users.Add(user);
        context.Playlists.Add(playlist);

        await context.SaveChangesAsync();
    }

    private async Task AddUserAndPlaylists(AppUser user, IEnumerable<Playlist> playlists)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Users.Add(user);
        context.Playlists.AddRange(playlists);

        await context.SaveChangesAsync();
    }
}
