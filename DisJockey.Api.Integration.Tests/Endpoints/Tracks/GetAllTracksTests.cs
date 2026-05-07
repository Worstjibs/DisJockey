using DisJockey.Core;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Tracks;

public class GetAllTracksTests : IntegrationTestBase
{
    public GetAllTracksTests(DisJockeyApiFixture fixture) 
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetTracks_ReturnsPaginatedListOfTracks()
    {
        // Arrange
        ulong discordId = 123456789012345678;

        _httpClient.AsUser();

        var createdDate = new DateTime(2025, 1, 15);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "TestUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var tracks = Enumerable.Range(1, 10).Select(i => new Track
        {
            Title = $"Track {i}",
            YoutubeId = $"YoutubeId_{i}",
            CreatedOn = createdDate,
            TrackPlays =
            [
                new()
                {
                     User = user,
                     CreatedOn = createdDate,
                     LastPlayed = createdDate,
                     TrackPlayHistory =
                     [
                         new() { CreatedOn =  createdDate },
                     ],
                }
            ]
        }).ToArray();

        await AddUserAndTracks(user, tracks);

        var expectedItems = tracks.Skip(5).Take(5).Select(t => new TrackListDto
        {
            Title = t.Title,
            YoutubeId = t.YoutubeId,
            Users = [.. t.TrackPlays.Select(tp => new TrackPlayDto
            {
                DiscordId = tp.User.DiscordId,
                Username = tp.User.UserName,
                FirstPlayed = tp.CreatedOn,
                History = [.. tp.TrackPlayHistory.Select(h => new TrackPlayHistoryDto
                {
                    CreatedOn = h.CreatedOn
                })],
                LastPlayed = tp.LastPlayed,
                TimesPlayed = tp.TrackPlayHistory.Count,
            })],
            UserLikes = [],
            LastPlayed = createdDate,
            CreatedOn = t.CreatedOn
        }).ToList();

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<TrackListDto>>(
                                $"/api/tracks" +
                                    $"?{nameof(PaginationParams.PageNumber)}=2" +
                                    $"&{nameof(PaginationParams.PageSize)}=5", 
                                TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.Items.ShouldNotBeEmpty();

        result.Items.ShouldBeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task GetTracks_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.GetAsync("/api/tracks", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task AddUserAndTracks(AppUser user, IEnumerable<Track> tracks)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Users.Add(user);

        context.Tracks.AddRange(tracks);

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
