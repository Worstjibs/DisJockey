using DisJockey.Core;
using System.Net;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class BlacklistTrackTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public BlacklistTrackTests(DisJockeyApiFixture fixture) : base(fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task BlacklistTrack_GivenSuccessfulRequest_ReturnsNoContent()
    {
        // Arrange
        var track = new Track
        {
            YoutubeId = "ToBlacklist",
            Title = "ToBlacklist",
            CreatedOn = DateTime.UtcNow
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
