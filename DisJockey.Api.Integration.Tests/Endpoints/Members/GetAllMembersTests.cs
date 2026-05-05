using DisJockey.Core;
using DisJockey.Infrastructure.Persistence;
using DisJockey.Shared.DTOs.Member;
using DisJockey.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Members;

public class GetAllMembersTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public GetAllMembersTests(DisJockeyApiFixture fixture)
        : base(fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task GetAllMembers_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.GetAsync("/api/members", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllMembers_ReturnsOkWithMembers()
    {
        // Arrange
        var createdDate = new DateTime(2025, 1, 15);

        var users = new[]
        {
            new AppUser
            {
                CreatedOn = createdDate,
                UserName = "UserOne",
                DiscordId = 111111111111111111,
                AvatarUrl = "http://example.com/avatar1.png"
            },
            new AppUser
            {
                CreatedOn = createdDate,
                UserName = "UserTwo",
                DiscordId = 222222222222222222,
                AvatarUrl = "http://example.com/avatar2.png"
            }
        };

        await InsertEntitiesAsync(users);

        _httpClient.AsUser();

        var expectedItems = users.Select(u => new MemberListDto
        {
            DiscordId = u.DiscordId.ToString(),
            Username = u.UserName,
            AvatarUrl = u.AvatarUrl,
            DateJoined = u.CreatedOn,
            TracksPlayed = 0
        }).ToList();

        // Act
        var result = await _httpClient.GetFromJsonAsync<IEnumerable<MemberListDto>>("/api/members", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result!.ShouldNotBeEmpty();
        result.ShouldBeEquivalentTo(expectedItems);
    }
}
