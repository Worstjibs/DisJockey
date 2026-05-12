#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class LavalinkResource : ContainerResource, IResourceWithConnectionString
{
    public LavalinkResource(string name) : base(name)
    {
        PrimaryEndpoint = new(this, "http");
    }

    /// <summary>
    /// Gets the primary endpoint for the Lavalink server.
    /// </summary>
    public EndpointReference PrimaryEndpoint { get; }

    public ReferenceExpression ConnectionStringExpression => UriExpression;

    /// <summary>
    /// Gets the connection URI expression for the Lavalink server.
    /// </summary>
    /// <remarks>
    /// Format: <c>http://localhost:{port}</c>.
    /// </remarks>
    public ReferenceExpression UriExpression
    {
        get
        {
            var builder = new ReferenceExpressionBuilder();
            builder.AppendLiteral("http://");
            builder.Append($"{PrimaryEndpoint.Property(EndpointProperty.HostAndPort)}");

            return builder.Build();
        }
    }
}
