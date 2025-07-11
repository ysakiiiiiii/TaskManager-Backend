using TaskManagerBackend.Models.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManagerBackend.DTOs.Attachment;
using TaskManagerBackend.DTOs.CheckList;

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
        public Task UpdateTaskAssignmentsAsync(int taskId, List<string> assignedUserIds);
        public Task UpdateTaskChecklistItemsAsync(int taskId, List<CheckListDto> checklistItems);

        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(TaskItem task);
        Task<bool> UpdateStatusAsync(int taskId, string statusName);
        Task<bool?> DeleteTaskAsync(TaskItem task);

    }
}
