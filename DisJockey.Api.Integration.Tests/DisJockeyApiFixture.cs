using DisJockey.Api.Integration.Tests;
using DisJockey.Application.Interfaces;
using DisJockey.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Respawn;
using System.Data;
using System.Data.Common;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

[assembly: AssemblyFixture(typeof(DisJockeyApiFixture))]

namespace DisJockey.Api.Integration.Tests;

public class DisJockeyApiFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const string _sqlServerImage = "mcr.microsoft.com/mssql/server:2022-latest";
    private const string _rabbitMqImage = "library/rabbitmq:4.2";

    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder(_sqlServerImage)
                                                    .WithPassword("Super secret password!")
                                                    .WithPortBinding(14333, 1433)
                                                    .Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder(_rabbitMqImage).Build();

    private DbConnection _sqlConnection = default!;
    private Respawner _respawner = default!;

    public HttpClient HttpClient { get; set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:disjockey-db", _sqlContainer.GetConnectionString());
        builder.UseSetting("ConnectionStrings:rabbit-mq", _rabbitMqContainer.GetConnectionString());

        builder.UseSetting("YoutubeSettings:ApiKey", "TestKey"); // Include to stop exceptions thrown on startup

        builder.ConfigureTestServices(services =>
        {
            services.ConfigureTestJwt();

            var discordAuthHandler = services.FirstOrDefault(x => x.ImplementationType == typeof(DisJockeyAuthorizationMiddlewareResultHandler));
            if (discordAuthHandler is not null)
            {
                services.Remove(discordAuthHandler);
            }

            services.RemoveAll<IVideoDetailService>();
            services.AddSingleton<IVideoDetailService>(FakeVideoDetailService);
        });
    }

    public FakeVideoDetailService FakeVideoDetailService { get; set; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Host = base.CreateHost(builder);

        return Host;
    }

    public IHost Host { get; set; } = default!;

    public async Task ResetDb()
        => await _respawner.ResetAsync(_sqlConnection);

    public async ValueTask InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        HttpClient = CreateClient();

        _sqlConnection = new SqlConnection(_sqlContainer.GetConnectionString());
        await _sqlConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_sqlConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"]
        });
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _sqlContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();

        await _sqlConnection.DisposeAsync();
    }
}
