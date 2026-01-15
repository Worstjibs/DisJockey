using DisJockey.Core;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Shared.DTOs.Track;
using DisJockey.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests;

public class TrackTests : IClassFixture<DisJockeyApiFixture>
{
    private readonly HttpClient _httpClient;
    private readonly DisJockeyApiFixture _fixture;

    public TrackTests(DisJockeyApiFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _fixture = fixture;
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

        var expectedItems = tracks.Select(t => new TrackListDto
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
        var result = await _httpClient.GetFromJsonAsync<PagedList<TrackListDto>>("/api/tracks", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.Items.ShouldNotBeEmpty();

        result.Items.ShouldBeEquivalentTo(expectedItems);
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
