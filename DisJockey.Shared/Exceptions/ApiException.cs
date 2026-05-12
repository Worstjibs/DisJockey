using System.Text.Json;

namespace DisJockey.Shared.Exceptions;

public class ApiException
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Details { get; set; }

    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApiException(
        int statusCode,
        string? message = null,
        string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, _options);
    }
}
