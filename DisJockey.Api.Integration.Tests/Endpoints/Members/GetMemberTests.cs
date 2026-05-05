using DisJockey.Core;
using DisJockey.Shared.DTOs.Member;
using System.Net;
using System.Net.Http.Json;

namespace DisJockey.Api.Integration.Tests.Endpoints.Members;

public class GetMemberTests : IntegrationTestBase
{
    private readonly HttpClient _httpClient;

    public GetMemberTests(DisJockeyApiFixture fixture) : base(fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task GetMember_GivenUnauthenticatedRequest_ReturnsUnauthorized()
    {
        // Arrange
        _httpClient.WithoutAuthentication();

        // Act
        var response = await _httpClient.GetAsync("/api/members/123456789012345678", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMember_GivenMemberDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _httpClient.AsUser();

        var missingDiscordId = 999999999999999999;

        // Act
        var response = await _httpClient.GetAsync($"/api/members/{missingDiscordId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMember_ReturnsOkWithMemberDetail()
    {
        // Arrange
        var createdDate = new DateTime(2025, 1, 15);

        var user = new AppUser
        {
            CreatedOn = createdDate,
            UserName = "UserOne",
            DiscordId = 111111111111111111,
            AvatarUrl = "http://example.com/avatar1.png"
        };

        await InsertEntityAsync(user);

        _httpClient.AsUser(user.DiscordId.ToString());

        var expected = new MemberDetailDto
        {
            DiscordId = user.DiscordId.ToString(),
            Username = user.UserName,
            AvatarUrl = user.AvatarUrl,
            DateJoined = user.CreatedOn,
            TracksPlayed = 0
        };

        // Act
        var result = await _httpClient.GetFromJsonAsync<MemberDetailDto>(
            $"/api/members/{user.DiscordId}", 
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEquivalentTo(expected);
    }
}
