namespace DisJockey.Application.Features.Search.Queries;

public class YouTubePagination(string currentPageToken, string nextPageToken, string previousPageToken)
{
    public string CurrentPageToken { get; set; } = currentPageToken;
    public string NextPageToken { get; set; } = nextPageToken;
    public string PreviousPageToken { get; set; } = previousPageToken;
}
