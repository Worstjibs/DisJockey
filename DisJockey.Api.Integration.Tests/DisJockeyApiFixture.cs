using DisJockey.Api.Integration.Tests;
using DisJockey.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(DisJockeyApiFixture))]

namespace DisJockey.Api.Integration.Tests;

public class DisJockeyApiFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const string _sqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder(_sqlServerImage).Build();

    public HttpClient HttpClient { get; set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:disjockey-db", _sqlContainer.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.ConfigureTestJwt();

            var discordAuthHandler = services.FirstOrDefault(x => x.ImplementationType == typeof(DisJockeyAuthorizationMiddlewareResultHandler));
            if (discordAuthHandler is not null)
            {
                services.Remove(discordAuthHandler);
            }
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _sqlContainer.StartAsync();

        HttpClient = CreateClient();
    }
}
