namespace TaskManagerBackend.DTOs.CheckList
{
    public class ChecklistItemDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Description { get; set; }
    }
}
