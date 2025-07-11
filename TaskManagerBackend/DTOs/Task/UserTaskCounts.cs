namespace TaskManagerBackend.DTOs.Task
{
    public class UserTaskCounts
    {
        public List<TaskStatusCount> StatusCounts { get; set; } = new();
        public List<TaskPriorityCount> PriorityCounts { get; set; } = new();
        public List<TaskCategoryCount> CategoryCounts { get; set; } = new();
    }

}
