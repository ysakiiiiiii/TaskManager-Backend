using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> GetTaskByIdAsync(int id);

    }
}
