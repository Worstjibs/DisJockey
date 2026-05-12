#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class LavalinkExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<LavalinkResource> AddLavalinkServer(
            string name,
            int port = 2333)
        {
            var lavalink = new LavalinkResource(name);

            var lavalinkBuilder = builder.AddResource(lavalink)
                    .WithImage("lavalink-devs/lavalink", "4.2.2")
                    .WithImageRegistry("ghcr.io")
                    .WithEndpoint(port, 2333, scheme: "http")
                    .WithBindMount("../application.yml", "/opt/Lavalink/application.yml");

            return lavalinkBuilder;
        }
    }
}
