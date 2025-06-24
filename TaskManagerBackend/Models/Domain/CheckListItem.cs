namespace TaskManagerBackend.Models.Domain
{
    public class ChecklistItem
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public Task Task { get; set; }

        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
