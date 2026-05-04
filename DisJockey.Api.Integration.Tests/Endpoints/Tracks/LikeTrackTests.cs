using DisJockey.Core;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class LikeTrackTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public LikeTrackTests(DisJockeyApiFixture fixture) : base(fixture)
    {
        _httpClient = fixture.HttpClient;
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
