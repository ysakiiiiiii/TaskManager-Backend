using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskItem> CreateAsync(TaskItem taskItem);
    }
}
