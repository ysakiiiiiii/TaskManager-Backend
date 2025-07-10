using TaskManagerBackend.DTOs.User;

namespace TaskManagerBackend.DTOs.SearchFilters
{
    public class PaginatedUserResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<UserDto> Items { get; set; } = new();
    }
}
