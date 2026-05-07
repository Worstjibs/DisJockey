using DisJockey.Core;
using DisJockey.Shared.Exceptions;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class LikeTrackTests : IntegrationTestBase
{
    public LikeTrackTests(DisJockeyApiFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task LikeTrack_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        var command = new { YouTubeId = "AnyTrack", Liked = true };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/like", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LikeTrack_GivenTrackDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _httpClient.AsUser();

        var command = new
        {
            YouTubeId = "NonExistentTrack",
            Liked = true
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/like", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LikeTrack_GivenUserDoesNotExist_ThrowsException()
    {
        // Arrange
        ulong discordId = 999999999999999998;

        _httpClient.AsUser(discordId.ToString());

        var youTubeId = "YoutubeId_NoUser";

        var track = new Track
        {
            YoutubeId = youTubeId
        };

        await InsertEntityAsync(track);

        var command = new
        {
            YouTubeId = youTubeId,
            Liked = true
        };

        // Act
        var action = () => _httpClient.PostAsJsonAsync("/api/tracks/like", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        await action.ShouldThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task LikeTrack_GivenTrackIsAlreadyLikedByUser_ReturnsConflict()
    {
        // Arrange
        ulong discordId = 999999999999999999;

        _httpClient.AsUser(discordId.ToString());

        var user = new AppUser
        {
            UserName = "UserAlreadyLiked",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        await InsertEntityAsync(user);

        var youTubeId = "YoutubeId_AlreadyLiked";

        var track = new Track
        {
            YoutubeId = youTubeId,
            Likes = [new TrackLike { UserId = user.Id, Liked = true }]
        };

        await InsertEntityAsync(track);

        var command = new
        {
            YouTubeId = youTubeId,
            Liked = true
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/like", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task LikeTrack_GivenSuccessfulRequest_ReturnsOk()
    {
        // Arrange
        ulong discordId = 123456789012345678;

        _httpClient.AsUser(discordId.ToString());

        var user = new AppUser
        {
            CreatedOn = DateTime.UtcNow,
            UserName = "User",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        await InsertEntityAsync(user);

        var youTubeId = "YoutubeId_1";

        var track = new Track
        {
            YoutubeId = youTubeId
        };

        await InsertEntityAsync(track);

        var command = new
        {
            YouTubeId = youTubeId,
            Liked = true
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/like", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
