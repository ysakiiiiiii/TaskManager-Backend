namespace TaskManagerBackend.DTOs.SearchFilters
{
    public class SearchFiltersDto
    {
        public List<BasicDto> Statuses { get; set; } = new();
        public List<BasicDto> Priorities { get; set; } = new();
        public List<BasicDto> Categories { get; set; } = new();
    }

}
