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

    public string SortBy { get; set; }
    public string Query { get; set; }
    public string PageToken { get; set; }
}