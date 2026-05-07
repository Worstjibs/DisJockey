using DisJockey.Core;
using System.Net;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class BlacklistTrackTests : IntegrationTestBase
{
    public BlacklistTrackTests(DisJockeyApiFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task BlacklistTrack_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.PutAsync(
                                $"/api/tracks/{int.MaxValue}/blacklist",
                                null,
                                TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BlacklistTrack_GivenTrackDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _httpClient.AsUser();

        // Act
        var response = await _httpClient.PutAsync(
                                $"/api/tracks/{int.MaxValue}/blacklist",
                                null,
                                TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BlacklistTrack_GivenTrackIsAlreadyBlacklisted_ReturnsConflict()
    {
        // Arrange
        var track = new Track
        {
            YoutubeId = "AlreadyBlacklisted",
            Title = "AlreadyBlacklisted",
            Blacklisted = true
        };

        _httpClient.AsUser();

        await InsertEntityAsync(track);

        // Act
        var response = await _httpClient.PutAsync(
                                $"/api/tracks/{track.Id}/blacklist",
                                null,
                                TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BlacklistTrack_GivenSuccessfulRequest_ReturnsNoContent()
    {
        // Arrange
        var track = new Track
        {
            YoutubeId = "ToBlacklist",
            Title = "ToBlacklist"
        };

        _httpClient.AsUser();

        await InsertEntityAsync(track);

        // Act
        var response = await _httpClient.PutAsync(
                                $"/api/tracks/{track.Id}/blacklist", 
                                null, 
                                TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
