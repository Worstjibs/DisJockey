using DisJockey.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace DisJockey.Api.Integration.Tests;

[Collection(SharedCollection.CollectionName)]
public class IntegrationTestBase : IAsyncLifetime
{
    protected readonly DisJockeyApiFixture _fixture;

    public IntegrationTestBase(DisJockeyApiFixture fixture)
    {
        _fixture = fixture;
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

    public async ValueTask DisposeAsync() => await _fixture.ResetDb();

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;
}
