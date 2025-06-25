namespace TaskManagerBackend.Models.Domain
{
    public class TaskAssignment
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
        public TaskItem Task { get; set; }

    }
}
