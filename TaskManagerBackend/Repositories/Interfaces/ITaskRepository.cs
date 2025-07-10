using TaskManagerBackend.Models.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TaskManagerBackend.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<IQueryable<TaskItem>> GetFilteredTasksQueryAsync(
            string? search,
            string? category,
            string? priority,
            string? status,
            string? sortBy,
            bool isAscending,
            string? userId,
            string? userRole,
            string? type);

        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(TaskItem task);
        Task<bool?> DeleteTaskAsync(TaskItem task);
    }
}
