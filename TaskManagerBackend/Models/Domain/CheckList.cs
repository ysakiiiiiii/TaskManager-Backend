namespace TaskManagerBackend.Models.Domain
{
    public class CheckList
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        //Foreign Key
        public TaskItem Task { get; set; }

    }
}
