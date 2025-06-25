using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem?> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> DeleteTaskAsync(int id);


    }
}
