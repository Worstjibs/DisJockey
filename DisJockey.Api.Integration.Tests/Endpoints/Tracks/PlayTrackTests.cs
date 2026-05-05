using DisJockey.Application.Features.Tracks.Commands.PlayTrack;
using DisJockey.Core;
using DisJockey.Shared.Helpers;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class PlayTrackTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public PlayTrackTests(DisJockeyApiFixture fixture) : base(fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task PlayTrack_GivenSuccessfulRequest_ReturnsOk()
    {
        // Arrange
        ulong discordId = 123456789012345679;

        _httpClient.AsUser(discordId.ToString());

        var user = new AppUser
        {
            CreatedOn = DateTime.UtcNow,
            UserName = "User1",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar1.png"
        };

        await InsertEntityAsync(user);

        var youtubeId = "YoutubeId_1";

        var track = new Track
        {
            YoutubeId = youtubeId,
            Title = "ToPlay",
            CreatedOn = DateTime.UtcNow
        };

        await InsertEntityAsync(track);

        _httpClient.AsUser();

        var command = new PlayTrackCommand(youtubeId, 0, true);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/play", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PlayTrack_GivenTrackIsBlacklisted_ReturnsBadRequest()
    {
        // Arrange
        var youtubeId = "BlacklistedTrack";

        var track = new Track
        {
            YoutubeId = youtubeId,
            Title = "BlacklistedTrack",
            Blacklisted = true
        };

        await InsertEntityAsync(track);

        _httpClient.AsUser();

        var command = new PlayTrackCommand(youtubeId, 0, true);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/play", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
