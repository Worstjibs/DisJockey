using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DisJockey.Extensions {
    public static class HttpExtensions {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalPages, int totalItems) {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalPages, totalItems);

            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}