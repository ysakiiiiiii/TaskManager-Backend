using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;

public interface ITaskService
{
    Task<List<TaskDto>> GetAllTasksAsync();
    Task<TaskDto?> GetTaskByIdAsync(int taskId);
    Task<TaskDto> CreateTaskAsync(AddTaskRequestDto dto, string userId);
    Task<TaskDto?> UpdateTaskAsync(int taskId, UpdateTaskRequestDto dto, string userId);
    Task<bool?> DeleteTaskAsync(int taskId, string userId);
}
