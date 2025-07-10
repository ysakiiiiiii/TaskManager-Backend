using TaskManagerBackend.DTOs.Task;

namespace TaskManagerBackend.DTOs.SearchFilters
{
    public class PaginatedTaskResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<TaskDto> Items { get; set; } = new();
    }
}
