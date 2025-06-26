using TaskManagerBackend.DTOs.Task;
using TaskManagerBackend.Models.Domain;

public interface ITaskService
{
    Task<List<TaskDto>> GetAllTasksAsync();
    Task<TaskDto?> GetTaskByIdAsync(int id);
    Task<TaskDto> CreateTaskAsync(AddTaskRequestDto dto, string userId);
    Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskRequestDto dto, string userId);
    Task<bool> DeleteTaskAsync(int id);
}
