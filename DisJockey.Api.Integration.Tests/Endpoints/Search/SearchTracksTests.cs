using DisJockey.Core;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Search;

public class SearchTracksTests : IntegrationTestBase
{
    public SearchTracksTests(DisJockeyApiFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task SearchTracks_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.GetAsync("/api/search", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SearchTracks_GivenNoResults_ReturnsEmptyListWithNoPaginationHeader()
    {
        // Arrange
        _httpClient.AsUser();
        _fixture.FakeVideoDetailService.QueryResult = YouTubePagedList<Track>.Empty();

        // Act
        var response = await _httpClient.GetAsync("/api/search?query=nothing", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<IEnumerable<TrackListDto>>(TestContext.Current.CancellationToken);
        body.ShouldNotBeNull();
        body.ShouldBeEmpty();

        response.Headers.Contains("Pagination").ShouldBeFalse();
    }

    [Fact]
    public async Task SearchTracks_GivenResults_ReturnsTracksWithPaginationHeader()
    {
        // Arrange
        _httpClient.AsUser();

        var youtubeTrack = new Track
        {
            YoutubeId = "yt_search_1",
            Title = "Search Result Track",
            Description = "A description",
            ChannelTitle = "Test Channel",
            SmallThumbnail = "http://example.com/small.jpg",
            MediumThumbnail = "http://example.com/medium.jpg",
            LargeThumbnail = "http://example.com/large.jpg",
            Likes = [],
            TrackPlays = []
        };

        _fixture.FakeVideoDetailService.QueryResult = new YouTubePagedList<Track>(
            [youtubeTrack],
            currentPageToken: "current",
            nextPageToken: "next",
            previousPageToken: "prev");

        // Act
        var response = await _httpClient.GetAsync("/api/search?query=test", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var results = await response.Content.ReadFromJsonAsync<IEnumerable<TrackListDto>>(TestContext.Current.CancellationToken);
        results.ShouldNotBeNull();

        var track = results.ShouldHaveSingleItem();
        track.YoutubeId.ShouldBe("yt_search_1");
        track.Title.ShouldBe("Search Result Track");

        response.Headers.Contains("Pagination").ShouldBeTrue();
    }

    [Fact]
    public async Task SearchTracks_GivenTrackAlreadyInDatabase_ReturnsDatabaseVersion()
    {
        // Arrange
        _httpClient.AsUser();

        var youtubeId = "yt_existing";

        var dbTrack = new Track
        {
            YoutubeId = youtubeId,
            Title = "DB Title",
            Description = "DB Description",
            ChannelTitle = "DB Channel",
            CreatedOn = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Likes = [],
            TrackPlays = []
        };

        await InsertEntityAsync(dbTrack);

        var youtubeTrack = new Track
        {
            YoutubeId = youtubeId,
            Title = "YouTube Title",
            Description = "YouTube Description",
            ChannelTitle = "YouTube Channel",
            Likes = [],
            TrackPlays = []
        };

        _fixture.FakeVideoDetailService.QueryResult = new YouTubePagedList<Track>(
            [youtubeTrack],
            currentPageToken: "current",
            nextPageToken: "next",
            previousPageToken: "prev");

        // Act
        var response = await _httpClient.GetAsync("/api/search?query=existing", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var results = await response.Content.ReadFromJsonAsync<IEnumerable<TrackListDto>>(TestContext.Current.CancellationToken);
        results.ShouldNotBeNull();

        var track = results.ShouldHaveSingleItem();

        // DB record should take precedence over YouTube data
        track.YoutubeId.ShouldBe(youtubeId);
        track.Title.ShouldBe("DB Title");
    }
}
