using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DisJockey.Shared.Helpers {
    public class YouTubePaginationHeader {
        public string CurrentPageToken { get; set; }
        public string NextPageToken { get; set; }
        public string PreviousPageToken { get; set; }

        public YouTubePaginationHeader(string currentPageToken, string nextPageToken, string previousPageToken) {
            CurrentPageToken = currentPageToken;
            NextPageToken = nextPageToken;
            PreviousPageToken = previousPageToken;
        }

        public override string ToString() {
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(this, options);
        }
    }
}
