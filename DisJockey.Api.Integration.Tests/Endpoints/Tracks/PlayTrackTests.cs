using DisJockey.Application.Features.Tracks.Commands.PlayTrack;
using DisJockey.Core;
using DisJockey.Shared.Helpers;
using DisJockey.Shared.Messaging.Events;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Wolverine.Tracking;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class PlayTrackTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public PlayTrackTests(DisJockeyApiFixture fixture) : base(fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task PlayTrack_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        var command = new PlayTrackCommand("YoutubeId_1", 0, true);

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/tracks/play", command, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
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
    public async Task PlayTrack_GivenSuccessfulRequest_PublishesPlayTrackEvent()
    {
        // Arrange
        ulong discordId = 123456789012345680;

        _httpClient.AsUser(discordId.ToString());

        var user = new AppUser
        {
            UserName = "User2",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar2.png"
        };

        await InsertEntityAsync(user);

        var youtubeId = "YoutubeId_ForEvent";

        var track = new Track
        {
            YoutubeId = youtubeId,
            Title = "ToPublish"
        };

        await InsertEntityAsync(track);

        var command = new PlayTrackCommand(youtubeId, 0, true);

        // Act
        var tracked = await _fixture.Host.ExecuteAndWaitAsync(async () =>
        {
            await _httpClient.PostAsJsonAsync("/api/tracks/play", command, cancellationToken: TestContext.Current.CancellationToken);
        });

        // Assert
        var @event = tracked.Sent.SingleMessage<PlayTrackEvent>();
        @event.YoutubeId.ShouldBe(youtubeId);
        @event.DiscordId.ShouldBe(discordId);
        @event.Queue.ShouldBeFalse();
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
