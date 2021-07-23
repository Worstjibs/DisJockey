namespace DisJockey.Shared.Helpers {
    public class PaginationHeader {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public PaginationHeader(int currentPage, int itemsPerPage, int totalPages, int totalItems) {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalPages = totalPages;
            TotalItems = totalItems;
        }
    }
}