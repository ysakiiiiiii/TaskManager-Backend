namespace TaskManagerBackend.DTOs.Task
{
    public record TaskStatusCount
    {
        public int StatusId { get; init; }
        public int Count { get; init; }
    }

}
