namespace API.Helpers {
    public class PaginationHeader {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public PaginationHeader(int currentPage, int pageSize, int totalPages, int totalItems) {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalItems = totalItems;
        }
    }
}