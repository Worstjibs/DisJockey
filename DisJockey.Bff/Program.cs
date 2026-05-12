using DisJockey.Bff.Endpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddProcessor(new WildcardRouteActivityProcessor()));

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddKeycloakOpenIdConnect(
        serviceName: "keycloak",
        realm: "master",
        options =>
        {
            options.RequireHttpsMetadata = false;

            options.SkipUnrecognizedRequests = true;

            options.ClientId = builder.Configuration["ClientId"];
            options.ClientSecret = builder.Configuration["ClientSecret"];
            options.ResponseType = OpenIdConnectResponseType.Code;

            options.SaveTokens = true;

            options.Scope.Add("profile"); 
            
            var keycloakPublicUrl = builder.Configuration["KeycloakPublicUrl"];
            if (string.IsNullOrEmpty(keycloakPublicUrl))
            {
                return;
            }

            options.TokenValidationParameters.ValidIssuer = $"{keycloakPublicUrl}/realms/master";

            options.Events.OnRedirectToIdentityProvider = context =>
            {
                var keycloakPublicUrl = builder.Configuration["KeycloakPublicUrl"];
                if (string.IsNullOrEmpty(keycloakPublicUrl))
                {
                    return Task.CompletedTask;
                }

                var issuerUri = new Uri(context.ProtocolMessage.IssuerAddress);
                context.ProtocolMessage.IssuerAddress = $"{keycloakPublicUrl}{issuerUri.AbsolutePath}{issuerUri.Query}";
                return Task.CompletedTask;
            };
        });

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.MapAuthEndpoints();

app.MapForwarder("/api/{**path}", "https+http://api", context =>
{
    context.AddRequestTransform(async transformContext =>
    {
        var token = await transformContext.HttpContext.GetTokenAsync("access_token");
        if (!string.IsNullOrEmpty(token))
        {
            transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    });
});

app.MapForwarder("/hubs/{**path}", "https+http://api", context =>
{
    context.AddRequestTransform(async transformContext =>
    {
        var token = await transformContext.HttpContext.GetTokenAsync("access_token");
        if (!string.IsNullOrEmpty(token))
        {
            transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    });
});


if (app.Environment.IsDevelopment())
{
    app.MapReverseProxy();
}
else
{
    app.MapFallbackToFile("index.html");
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Replaces the wildcard route template in span display names with the actual request path.
// Without this, MapForwarder("/api/{**path}") always shows as "GET /api/{**path}" in traces.
sealed class WildcardRouteActivityProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        if (activity.GetTagItem("http.route") is string route && route.Contains("{**") &&
            activity.GetTagItem("url.path") is string path)
        {
            activity.DisplayName = $"{activity.GetTagItem("http.request.method")} {path}";
        }
    }
}
