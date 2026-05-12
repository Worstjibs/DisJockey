namespace DisJockey.Shared.Helpers;

public class PaginationParams
{
    private const int _maxPageSize = 50;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => field;
        set => field = (value > _maxPageSize) ? _maxPageSize : value;
    } = 10;

    public string? SortBy { get; set; }
    public string? Query { get; set; }
    public string? PageToken { get; set; }


    public static PaginationParams CreateParameters(
        int? pageNumber = null,
        int? pageSize = null,
        string? sortBy = null,
        string? query = null,
        string? pageToken = null)
    {
        var pagination = new PaginationParams
        {
            SortBy = sortBy,
            Query = query,
            PageToken = pageToken
        };

        if (pageNumber.HasValue)
        {
            pagination.PageNumber = pageNumber.Value;
        }

        if (pageSize.HasValue)
        {
            pagination.PageSize = pageSize.Value;
        }   

        return pagination;
    }
}