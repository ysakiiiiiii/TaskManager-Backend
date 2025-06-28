using TaskManagerBackend.Models.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TaskManagerBackend.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(TaskItem task);
        Task<bool?> DeleteTaskAsync(TaskItem task);
    }
}
