using TaskManagerBackend.DTOs.SearchFilters;
using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;

public interface ITaskService
{
    Task<PaginatedTaskResponse> GetAllTasksAsync(TaskQueryParameters parameters, string? userId, string? userRole);
    Task<TaskDto?> GetTaskByIdAsync(int taskId);
    Task<TaskDto> CreateTaskAsync(AddTaskRequestDto dto, string userId);
    Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskRequestDto dto, string userId);
    Task DeleteTaskAsync(int taskId, string userId);
}
