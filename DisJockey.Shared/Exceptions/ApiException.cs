using System.Text.Json;

namespace DisJockey.Shared.Exceptions {
    public class ApiException {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        private readonly JsonSerializerOptions _options;

        public ApiException(int statusCode, string message = null, string details = null) {
            StatusCode = statusCode;
            Message = message;
            Details = details;

            _options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public override string ToString() {
            return JsonSerializer.Serialize(this, _options);
        }
    }
}
