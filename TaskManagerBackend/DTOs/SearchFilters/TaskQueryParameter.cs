namespace TaskManagerBackend.DTOs.SearchFilters
{
    public class TaskQueryParameters
    {
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? Type { get; set; }
    }

}
