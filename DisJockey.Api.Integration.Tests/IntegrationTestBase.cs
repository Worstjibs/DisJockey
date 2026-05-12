using DisJockey.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Api.Integration.Tests;

[Collection(SharedCollection.CollectionName)]
public class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient _httpClient;
    protected readonly DisJockeyApiFixture _fixture;

    public IntegrationTestBase(DisJockeyApiFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.HttpClient;
    }

    public async Task InsertEntityAsync<T>(T entity) where T : class
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Set<T>().Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task InsertEntitiesAsync<T>(IEnumerable<T> entities) where T : class
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Set<T>().AddRange(entities);

        await context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _fixture.ResetDb();
        _fixture.FakeVideoDetailService.Reset();
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;
}
