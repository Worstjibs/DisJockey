using DisJockey.Core;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Shared.DTOs.PullUps;
using DisJockey.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.PullUps;

public class GetPullUpsForMemberTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public GetPullUpsForMemberTests(DisJockeyApiFixture fixture) : base(fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.GetAsync(
            "/api/pullups/123456789012345678",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenNoTracksForMember_ReturnsEmptyPagedList()
    {
        // Arrange
        ulong discordId = 100000000000000001;

        _httpClient.AsUser(discordId.ToString());

        var user = new AppUser
        {
            CreatedOn = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            UserName = "NoPullUpsUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        await InsertEntityAsync(user);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    [Fact]
    public async Task GetPullUpsForMember_ReturnsPaginatedResultWithCorrectItems()
    {
        // Arrange
        ulong discordId = 100000000000000002;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 3, 10, 12, 0, 0, DateTimeKind.Utc);
        var pullUpDate1 = new DateTime(2025, 3, 11, 9, 0, 0, DateTimeKind.Utc);
        var pullUpDate2 = new DateTime(2025, 3, 12, 14, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "PullUpUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var track = new Track
        {
            YoutubeId = "yt_happy_path",
            Title = "Happy Path Track",
            Description = "A track description",
            ChannelTitle = "Some Channel",
            CreatedOn = createdDate,
            SmallThumbnail = "http://example.com/small.jpg",
            MediumThumbnail = "http://example.com/medium.jpg",
            LargeThumbnail = "http://example.com/large.jpg",
            Likes = [],
            PullUps =
            [
                new PullUp { User = user, CreatedOn = pullUpDate1, TimePulled = 42.5 },
                new PullUp { User = user, CreatedOn = pullUpDate2, TimePulled = 88.0 }
            ]
        };

        await AddUserAndTrack(user, track);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();

        var item = result.Items.Single();

        item.YoutubeId.ShouldBe(track.YoutubeId);
        item.Title.ShouldBe(track.Title);
        item.Description.ShouldBe(track.Description);
        item.ChannelTitle.ShouldBe(track.ChannelTitle);
        item.CreatedOn.ShouldBe(createdDate);
        item.SmallThumbnail.ShouldBe(track.SmallThumbnail);
        item.MediumThumbnail.ShouldBe(track.MediumThumbnail);
        item.LargeThumbnail.ShouldBe(track.LargeThumbnail);
        item.Likes.ShouldBe(0);
        item.Dislikes.ShouldBe(0);
        item.LikedByUser.ShouldBeNull();

        item.LastPulled.ShouldBe(pullUpDate2); // Max of the two pull-up dates

        item.PullUps.ShouldNotBeNull();
        item.PullUps.Count.ShouldBe(2);

        var orderedPullUps = item.PullUps.OrderBy(p => p.DatePulled).ToList();

        orderedPullUps[0].DatePulled.ShouldBe(pullUpDate1);
        orderedPullUps[0].TimePulled.ShouldBe(42.5);
        orderedPullUps[0].Member.ShouldBeNull(); // PullUp.User is not mapped to PullUpTrackDto.Member

        orderedPullUps[1].DatePulled.ShouldBe(pullUpDate2);
        orderedPullUps[1].TimePulled.ShouldBe(88.0);
        orderedPullUps[1].Member.ShouldBeNull();
    }

    [Fact]
    public async Task GetPullUpsForMember_DoesNotReturnTracksForOtherMembers()
    {
        // Arrange
        ulong targetDiscordId = 100000000000000003;
        ulong otherDiscordId = 100000000000000004;

        _httpClient.AsUser(targetDiscordId.ToString());

        var createdDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc);

        var targetUser = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "TargetUser",
            DiscordId = targetDiscordId,
            AvatarUrl = "http://example.com/target.png"
        };

        var otherUser = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "OtherUser",
            DiscordId = otherDiscordId,
            AvatarUrl = "http://example.com/other.png"
        };

        var targetTrack = new Track
        {
            YoutubeId = "yt_target_only",
            Title = "Target Member Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = targetUser, CreatedOn = createdDate, TimePulled = 10.0 }]
        };

        var otherTrack = new Track
        {
            YoutubeId = "yt_other_only",
            Title = "Other Member Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = otherUser, CreatedOn = createdDate, TimePulled = 20.0 }]
        };

        await AddUsersAndTracks([targetUser, otherUser], [targetTrack, otherTrack]);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{targetDiscordId}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.TotalItems.ShouldBe(1);
        result.Items.Single().YoutubeId.ShouldBe("yt_target_only");
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenBlacklistedTrack_ExcludesItFromResults()
    {
        // Arrange
        ulong discordId = 100000000000000005;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 4, 5, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "BlacklistTestUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var visibleTrack = new Track
        {
            YoutubeId = "yt_visible",
            Title = "Visible Track",
            CreatedOn = createdDate,
            Blacklisted = false,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate, TimePulled = 1.0 }]
        };

        var blacklistedTrack = new Track
        {
            YoutubeId = "yt_blacklisted",
            Title = "Blacklisted Track",
            CreatedOn = createdDate,
            Blacklisted = true,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate, TimePulled = 2.0 }]
        };

        await AddUserAndTracks(user, [visibleTrack, blacklistedTrack]);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.TotalItems.ShouldBe(1);
        result.Items.Single().YoutubeId.ShouldBe("yt_visible");
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenPageNumberAndPageSize_ReturnsPaginatedMetadata()
    {
        // Arrange
        ulong discordId = 100000000000000006;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "PaginationUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var tracks = Enumerable.Range(1, 7).Select(i => new Track
        {
            YoutubeId = $"yt_page_{i}",
            Title = $"Pagination Track {i}",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(i), TimePulled = i * 1.0 }]
        }).ToArray();

        await AddUserAndTracks(user, tracks);

        // Act — page 2 of 3 (7 total items, page size 3)
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}?pageNumber=2&pageSize=3",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.CurrentPage.ShouldBe(2);
        result.ItemsPerPage.ShouldBe(3);
        result.TotalItems.ShouldBe(7);
        result.TotalPages.ShouldBe(3);
        result.Items.Count().ShouldBe(3);
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenSortByTitle_ReturnsTitleAscending()
    {
        // Arrange
        ulong discordId = 100000000000000007;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "SortTitleUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        // Seeded deliberately out of alphabetical order
        var tracks = new[]
        {
            new Track
            {
                YoutubeId = "yt_sort_c",
                Title = "Charlie Track",
                CreatedOn = createdDate,
                Likes = [],
                PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(1), TimePulled = 1.0 }]
            },
            new Track
            {
                YoutubeId = "yt_sort_a",
                Title = "Alpha Track",
                CreatedOn = createdDate,
                Likes = [],
                PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(2), TimePulled = 2.0 }]
            },
            new Track
            {
                YoutubeId = "yt_sort_b",
                Title = "Bravo Track",
                CreatedOn = createdDate,
                Likes = [],
                PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(3), TimePulled = 3.0 }]
            }
        };

        await AddUserAndTracks(user, tracks);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}?sortBy=title",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Select(x => x.YoutubeId).ShouldBe(["yt_sort_a", "yt_sort_b", "yt_sort_c"]);
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenSortByFirstPulled_ReturnsLastPulledAscending()
    {
        // Arrange
        ulong discordId = 100000000000000008;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "SortFirstPulledUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        // Seeded deliberately out of chronological order
        var late = new Track
        {
            YoutubeId = "yt_late",
            Title = "Late Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(10), TimePulled = 1.0 }]
        };

        var early = new Track
        {
            YoutubeId = "yt_early",
            Title = "Early Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(1), TimePulled = 2.0 }]
        };

        var mid = new Track
        {
            YoutubeId = "yt_mid",
            Title = "Mid Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(5), TimePulled = 3.0 }]
        };

        await AddUserAndTracks(user, [late, early, mid]);

        // Act
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}?sortBy=firstPulled",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Select(x => x.YoutubeId).ShouldBe(["yt_early", "yt_mid", "yt_late"]);
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenDefaultSort_ReturnsLastPulledDescending()
    {
        // Arrange
        ulong discordId = 100000000000000009;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "DefaultSortUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var early = new Track
        {
            YoutubeId = "yt_default_early",
            Title = "Early Default Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(1), TimePulled = 1.0 }]
        };

        var late = new Track
        {
            YoutubeId = "yt_default_late",
            Title = "Late Default Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(10), TimePulled = 2.0 }]
        };

        var mid = new Track
        {
            YoutubeId = "yt_default_mid",
            Title = "Mid Default Track",
            CreatedOn = createdDate,
            Likes = [],
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(5), TimePulled = 3.0 }]
        };

        await AddUserAndTracks(user, [early, late, mid]);

        // Act — no sortBy param → default descending by LastPulled
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Select(x => x.YoutubeId).ShouldBe(["yt_default_late", "yt_default_mid", "yt_default_early"]);
    }

    [Fact]
    public async Task GetPullUpsForMember_GivenLikedByUser_ReflectsAuthenticatedUserLikeStatus()
    {
        // Arrange
        ulong discordId = 100000000000000010;

        _httpClient.AsUser(discordId.ToString());

        var createdDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "LikeStatusUser",
            DiscordId = discordId,
            AvatarUrl = "http://example.com/avatar.png"
        };

        var likedTrack = new Track
        {
            YoutubeId = "yt_liked",
            Title = "Liked Track",
            CreatedOn = createdDate,
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(3), TimePulled = 1.0 }],
            Likes = [new TrackLike { User = user, Liked = true }]
        };

        var dislikedTrack = new Track
        {
            YoutubeId = "yt_disliked",
            Title = "Disliked Track",
            CreatedOn = createdDate,
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(2), TimePulled = 2.0 }],
            Likes = [new TrackLike { User = user, Liked = false }]
        };

        var neutralTrack = new Track
        {
            YoutubeId = "yt_neutral",
            Title = "Neutral Track",
            CreatedOn = createdDate,
            PullUps = [new PullUp { User = user, CreatedOn = createdDate.AddDays(1), TimePulled = 3.0 }],
            Likes = []
        };

        await AddUserAndTracks(user, [likedTrack, dislikedTrack, neutralTrack]);

        // Act — sort by title for deterministic ordering
        var result = await _httpClient.GetFromJsonAsync<PagedList<PullUpDto>>(
            $"/api/pullups/{discordId}?sortBy=title",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Single(x => x.YoutubeId == "yt_liked").LikedByUser.ShouldBe(true);
        result.Items.Single(x => x.YoutubeId == "yt_disliked").LikedByUser.ShouldBe(false);
        result.Items.Single(x => x.YoutubeId == "yt_neutral").LikedByUser.ShouldBeNull();
    }

    private async Task AddUserAndTrack(AppUser user, Track track)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Users.Add(user);
        context.Tracks.Add(track);

        await context.SaveChangesAsync();
    }

    private async Task AddUserAndTracks(AppUser user, IEnumerable<Track> tracks)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Users.Add(user);
        context.Tracks.AddRange(tracks);

        await context.SaveChangesAsync();
    }

    private async Task AddUsersAndTracks(IEnumerable<AppUser> users, IEnumerable<Track> tracks)
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Users.AddRange(users);
        context.Tracks.AddRange(tracks);

        await context.SaveChangesAsync();
    }
}
