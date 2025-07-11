namespace TaskManagerBackend.DTOs.Task
{
    public record TaskStatusCount
    {
        public int Id { get; init; }
        public int Count { get; init; }
    }

}
