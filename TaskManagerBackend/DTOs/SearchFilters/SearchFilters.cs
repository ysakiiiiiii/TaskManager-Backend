namespace TaskManagerBackend.DTOs.SearchFilters
{
    public class SearchFiltersDto
    {
        public List<string> Statuses { get; set; } = new();
        public List<string> Priorities { get; set; } = new();
        public List<string> Categories { get; set; } = new();
    }

}
