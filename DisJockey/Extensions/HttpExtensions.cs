using DisJockey.Shared.Helpers;
using Microsoft.AspNetCore.Http;

namespace DisJockey.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalPages, int totalItems)
    {
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalPages, totalItems);

        response.Headers.Append("Pagination", paginationHeader.ToString());
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }

    public static void AddYouTubePaginationHeader(this HttpResponse response, string currentPageToken, string nextPageToken, string previousPageToken)
    {
        var paginationHeader = new YouTubePaginationHeader(currentPageToken, nextPageToken, previousPageToken);

        response.Headers.Append("Pagination", paginationHeader.ToString());
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}