using DisJockey.Core;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Shared.DTOs.Playlist;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Playlists;

public class AddPlaylistTests : IntegrationTestBase
{
    private readonly FakeVideoDetailService _fakeVideoDetailService;

    public AddPlaylistTests(DisJockeyApiFixture fixture) 
        : base(fixture)
    {
        _fakeVideoDetailService = fixture.FakeVideoDetailService;
    }

    [Fact]
    public async Task AddPlaylist_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLtest" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddPlaylist_GivenInvalidPlaylistId_ReturnsBadRequest()
    {
        // Arrange
        _httpClient.AsUser();

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLdoesnotexist" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddPlaylist_GivenPlaylistWithNoTracks_ReturnsBadRequest()
    {
        // Arrange
        _httpClient.AsUser();

        _fakeVideoDetailService.PlaylistResult = new Playlist
        {
            Name = "Empty Playlist",
            YoutubeId = "PLempty",
            Tracks = []
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLempty" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddPlaylist_GivenPlaylistAlreadyExists_ReturnsConflict()
    {
        // Arrange
        ulong discordId = 300000000000000001;
        _httpClient.AsUser(discordId.ToString());

        var existingPlaylist = new Playlist
        {
            Name = "Existing Playlist",
            YoutubeId = "PLexists",
            Tracks = []
        };

        await InsertEntityAsync(existingPlaylist);

        _fakeVideoDetailService.PlaylistResult = new Playlist
        {
            Name = "Existing Playlist",
            YoutubeId = "PLexists",
            Tracks = [new PlaylistTrack { Track = new Track { YoutubeId = "yt1", Title = "Track 1", CreatedOn = DateTime.UtcNow }, CreatedOn = DateTime.UtcNow }]
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLexists" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddPlaylist_GivenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        ulong discordId = 300000000000000002;
        _httpClient.AsUser(discordId.ToString());

        _fakeVideoDetailService.PlaylistResult = new Playlist
        {
            Name = "Some Playlist",
            YoutubeId = "PLnousercheck",
            Tracks = [new PlaylistTrack { Track = new Track { YoutubeId = "yt_nouser", Title = "Track", CreatedOn = DateTime.UtcNow }, CreatedOn = DateTime.UtcNow }]
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLnousercheck" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddPlaylist_GivenValidRequest_ReturnsPlaylistWithTracks()
    {
        // Arrange
        ulong discordId = 300000000000000003;
        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "PlaylistAdder",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        await InsertEntityAsync(user);

        _fakeVideoDetailService.PlaylistResult = new Playlist
        {
            Name = "New Playlist",
            YoutubeId = "PLnew001",
            Tracks =
            [
                new PlaylistTrack
                {
                    Track = new Track
                    {
                        YoutubeId = "yt_new_1",
                        Title = "New Track One",
                        Description = "Description 1",
                        ChannelTitle = "Channel 1",
                        CreatedOn = createdDate,
                        SmallThumbnail = "http://example.com/small.jpg",
                        MediumThumbnail = "http://example.com/medium.jpg",
                        LargeThumbnail = "http://example.com/large.jpg"
                    },
                    CreatedOn = createdDate
                },
                new PlaylistTrack
                {
                    Track = new Track
                    {
                        YoutubeId = "yt_new_2",
                        Title = "New Track Two",
                        Description = "Description 2",
                        ChannelTitle = "Channel 2",
                        CreatedOn = createdDate,
                        SmallThumbnail = "http://example.com/small.jpg",
                        MediumThumbnail = "http://example.com/medium.jpg",
                        LargeThumbnail = "http://example.com/large.jpg"
                    },
                    CreatedOn = createdDate
                }
            ]
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLnew001" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PlaylistDetailDto>(
            TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.YoutubeId.ShouldBe("PLnew001");
        result.Name.ShouldBe("New Playlist");
        result.Tracks.Count.ShouldBe(2);
        result.Tracks.Select(t => t.YoutubeId).ShouldContain("yt_new_1");
        result.Tracks.Select(t => t.YoutubeId).ShouldContain("yt_new_2");
    }

    [Fact]
    public async Task AddPlaylist_GivenValidRequest_PersistsPlaylistToDatabase()
    {
        // Arrange
        ulong discordId = 300000000000000004;
        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "PersistenceUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        await InsertEntityAsync(user);

        _fakeVideoDetailService.PlaylistResult = new Playlist
        {
            Name = "Persisted Playlist",
            YoutubeId = "PLpersist001",
            Tracks =
            [
                new PlaylistTrack
                {
                    Track = new Track
                    {
                        YoutubeId = "yt_persist_1",
                        Title = "Persisted Track",
                        Description = "Description",
                        ChannelTitle = "Channel",
                        CreatedOn = createdDate,
                        SmallThumbnail = "http://example.com/small.jpg",
                        MediumThumbnail = "http://example.com/medium.jpg",
                        LargeThumbnail = "http://example.com/large.jpg"
                    },
                    CreatedOn = createdDate
                }
            ]
        };

        // Act
        await _httpClient.PostAsJsonAsync(
            "/api/playlists",
            new { playlistId = "PLpersist001" },
            TestContext.Current.CancellationToken);

        // Assert
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        var savedPlaylist = context.Playlists
            .SingleOrDefault(p => p.YoutubeId == "PLpersist001");

        savedPlaylist.ShouldNotBeNull();
        savedPlaylist.Name.ShouldBe("Persisted Playlist");
    }
}
